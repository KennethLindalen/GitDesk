using GitDeskImport.Contexts;
using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GitDeskImport.Pages.Account;

[AllowAnonymous]
public class AcceptInviteModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AcceptInviteModel(AppDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty(SupportsGet = true)]
    public string Code { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        // Krev innlogging – redirect hvis ikke
        if (!_signInManager.IsSignedIn(User))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity", ReturnUrl = $"/Account/AcceptInvite?code={Code}" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        if (user.BusinessId != null)
        {
            return BadRequest("You are already connected to a business.");
        }

        var invite = await _context.BusinessInvites
            .FirstOrDefaultAsync(i => i.InviteCode == Code && !i.Used);

        if (invite == null)
        {
            return NotFound("This invite is no longer valid.");
        }

        // Koble bruker til bedrift + marker invitasjon brukt
        using var transaction = await _context.Database.BeginTransactionAsync();

        user.BusinessId = invite.BusinessModelId;
        invite.Used = true;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        TempData["InviteSuccess"] = true;
        return RedirectToPage("/Account/InviteAccepted");
    }
}

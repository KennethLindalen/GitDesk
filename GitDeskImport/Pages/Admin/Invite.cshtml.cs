using GitDeskImport.Contexts;
using GitDeskImport.Models.Business;
using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GitDeskImport.Pages.Admin
{
    [Authorize]
    public class InviteModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public InviteModel(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string? InviteLink { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var invite = new BusinessInvite
            {
                BusinessModelId = user.BusinessId,
            };
            _context.BusinessInvites.Add(invite);
            await _context.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            InviteLink = $"{baseUrl}/Account/AcceptInvite?code={invite.InviteCode}";

            return Page();
        }
    }

}

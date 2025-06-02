using GitDeskImport.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GitDeskImport.Mappers;
using GitDeskImport.Models.User;
using GitDeskImport.Services;
using Microsoft.AspNetCore.Identity;

namespace GitDeskImport.Pages.Mappings;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly TokenProtector _protector;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(AppDbContext context, TokenProtector protector, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _protector = protector;
        _userManager = userManager;
    }

    public List<SyncMapping> Mappings { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        Mappings = await _context.SyncMappings
            .Where(m => m.BusinessId == user.BusinessId)
            .OrderByDescending(m => m.LastSyncedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostSyncAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var mapping = await _context.SyncMappings
            .Include(m => m.Business)
            .FirstOrDefaultAsync(m => m.Id == id && m.BusinessId == user.BusinessId);

        if (mapping == null)
            return NotFound();

        mapping.Business.AttachProtector(_protector);

        var zendesk = new ZendeskService(mapping.Business.ZendeskApiToken, mapping.Business.ZendeskDomain);
        var github = new GitHubService(mapping.Business.GitHubToken);

        var ticket = await zendesk.GetTicket(mapping.ZendeskTicketId);
        await github.PostComment(mapping.GitHubRepoFullName, mapping.GitHubIssueNumber, $"Manual sync: {ticket.Subject}");

        mapping.LastSyncedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var mapping = await _context.SyncMappings
            .FirstOrDefaultAsync(m => m.Id == id && m.BusinessId == user.BusinessId);

        if (mapping == null)
            return NotFound();

        _context.SyncMappings.Remove(mapping);
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}

using GitDeskImport.Contexts;
using GitDeskImport.Mappers;
using GitDeskImport.Models;
using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardModel(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public List<SyncMapping> Mappings { get; set; } = new();
    public List<SyncConfiguration> Configurations { get; set; } = new();
    public bool IsGitHubConfigured { get; set; }
    public bool IsZendeskConfigured { get; set; }
    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.BusinessId == null) return Unauthorized();

        var config = await _context.Businesses
            .Where(c => c.Id == user.BusinessId)
            .ToListAsync();
        Mappings = await _context.SyncMappings
            .Where(m => m.BusinessId == user.BusinessId)
            .ToListAsync();

        IsGitHubConfigured = config.Any(c => !string.IsNullOrWhiteSpace(c.EncryptedGitHubToken));
        IsZendeskConfigured = config.Any(c => !string.IsNullOrWhiteSpace(c.EncryptedZendeskApiToken));

        return Page();
    }
}
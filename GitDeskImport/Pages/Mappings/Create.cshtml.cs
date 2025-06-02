using GitDeskImport.Contexts;
using GitDeskImport.Mappers;
using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GitDeskImport.Pages.Mappings;

[Authorize]
public class CreateModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public long ZendeskTicketId { get; set; }
        public string GitHubRepoFullName { get; set; } = string.Empty;
        public int GitHubIssueNumber { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = await _userManager.GetUserAsync(User);
        var mapping = new SyncMapping
        {
            BusinessId = user.BusinessId,
            ZendeskTicketId = Input.ZendeskTicketId,
            GitHubRepoFullName = Input.GitHubRepoFullName,
            GitHubIssueNumber = Input.GitHubIssueNumber
        };

        _context.SyncMappings.Add(mapping);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
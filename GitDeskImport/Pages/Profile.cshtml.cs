using GitDeskImport.Contexts;
using GitDeskImport.Models.User;
using GitDeskImport.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;
    private readonly TokenProtector _tokenProtector;

    public ProfileModel(UserManager<ApplicationUser> userManager, AppDbContext context, TokenProtector tokenProtector)
    {
        _userManager = userManager;
        _context = context;
        _tokenProtector = tokenProtector;
    }

    [BindProperty]
    public ProfileInput Input { get; set; }

    public class ProfileInput
    {
        public string Email { get; set; }
        public string ZendeskDomain { get; set; }
        public string ZendeskApiToken { get; set; }
        public string GitHubUsername { get; set; }
        public string GitHubToken { get; set; }
    }

    public async Task OnGetAsync()
    {
        var user = await _context.Users
            .Include(u => u.BusinessModel)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

        var business = user.BusinessModel;
        SetConnectionStatus(user);
        business.AttachProtector(_tokenProtector);

        Input = new ProfileInput
        {
            Email = user.Email,
            ZendeskDomain = business.ZendeskDomain,
            ZendeskApiToken = business.ZendeskApiToken,
            GitHubUsername = business.GitHubUsername,
            GitHubToken = business.GitHubToken
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _context.Users
            .Include(u => u.BusinessModel)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
        var business = user.BusinessModel;
        SetConnectionStatus(user);
        business.AttachProtector(_tokenProtector);

        user.Email = Input.Email;
        user.UserName = Input.Email;

        business.ZendeskDomain = Input.ZendeskDomain;
        business.ZendeskApiToken = Input.ZendeskApiToken;
        business.GitHubUsername = Input.GitHubUsername;
        business.GitHubToken = Input.GitHubToken;

        await _userManager.UpdateAsync(user);
        _context.Businesses.Update(business);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Profile updated successfully.";
        return RedirectToPage();
    }
    
    public bool IsZendeskConfigured { get; set; }
    public bool IsGitHubConfigured { get; set; }

    private void SetConnectionStatus(ApplicationUser user)
    {
        IsZendeskConfigured = !string.IsNullOrWhiteSpace(user.BusinessModel.EncryptedZendeskApiToken) &&
                              !string.IsNullOrWhiteSpace(user.BusinessModel.ZendeskDomain);

        IsGitHubConfigured = !string.IsNullOrWhiteSpace(user.BusinessModel.EncryptedGitHubToken) &&
                             !string.IsNullOrWhiteSpace(user.BusinessModel.GitHubUsername);
    }
}

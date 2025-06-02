using System.ComponentModel.DataAnnotations;
using GitDeskImport.Contexts;
using GitDeskImport.Models.User;
using GitDeskImport.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterBusinessModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenProtector _tokenProtector;

    public RegisterBusinessModel(
        AppDbContext context,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenProtector tokenProtector)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProtector = tokenProtector;
    }

    [BindProperty]
    public RegisterBusinessInput Input { get; set; }

    public class RegisterBusinessInput
    {
        [Required] public string BusinessName { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required][DataType(DataType.Password)] public string Password { get; set; }

        [Required] public string ZendeskDomain { get; set; }
        [Required] public string ZendeskApiToken { get; set; }

        [Required] public string GitHubUsername { get; set; }
        [Required] public string GitHubToken { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var business = new BusinessModel
        {
            Id = Guid.NewGuid(),
            Name = Input.BusinessName,
            ZendeskDomain = Input.ZendeskDomain,
            GitHubUsername = Input.GitHubUsername
        };

        business.AttachProtector(_tokenProtector);
        business.ZendeskApiToken = Input.ZendeskApiToken;
        business.GitHubToken = Input.GitHubToken;
        business.WebhookSecret = Guid.NewGuid().ToString("N");

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            Role = "Admin",
            BusinessModel = business
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
            return Page();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Dashboard");
    }
}

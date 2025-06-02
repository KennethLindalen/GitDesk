using GitDeskImport.Models;
using System.Text.Json;
using GitDeskImport.Contexts;
using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Step3Model : PageModel
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public Step3Model(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public WizardState State { get; set; } = new();

    public IActionResult OnGet()
    {
        if (!TempData.TryGetValue("WizardState", out var serialized))
            return RedirectToPage("Step1");

        State = JsonSerializer.Deserialize<WizardState>((string)serialized!)!;
        TempData.Keep("WizardState");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.BusinessId == null) return Unauthorized();

        var config = new SyncConfiguration
        {
            BusinessModelId = user.BusinessId,
            SourceSystem = State.SourceSystem,
            Repository = State.Repository,
            ZendeskDomain = State.ZendeskDomain,
            SyncFrequency = State.SyncFrequency
        };

        _context.SyncConfigurations.Add(config);
        await _context.SaveChangesAsync();

        TempData.Remove("WizardState");
        return RedirectToPage("/Mappings/Index");
    }
}
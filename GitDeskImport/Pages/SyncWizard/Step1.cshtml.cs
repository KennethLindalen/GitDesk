using System.Text.Json;
using GitDeskImport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Step1Model : PageModel
{
    [BindProperty]
    public string SourceSystem { get; set; } = "";

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(SourceSystem)) return Page();

        var state = new WizardState { SourceSystem = SourceSystem };
        TempData["WizardState"] = JsonSerializer.Serialize(state);

        return RedirectToPage("Step2");
    }
}
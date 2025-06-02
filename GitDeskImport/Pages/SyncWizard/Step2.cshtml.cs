using System.Text.Json;
using GitDeskImport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Step2Model : PageModel
{
    [BindProperty]
    public WizardState State { get; set; } = new();

    public IActionResult OnGet()
    {
        if (!TempData.TryGetValue("WizardState", out var serialized)) return RedirectToPage("Step1");

        State = JsonSerializer.Deserialize<WizardState>((string)serialized!)!;
        TempData.Keep("WizardState");

        return Page();
    }

    public IActionResult OnPost()
    {
        TempData["WizardState"] = JsonSerializer.Serialize(State);
        return RedirectToPage("Step3");
    }
}
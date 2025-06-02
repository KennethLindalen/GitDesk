using GitDeskImport.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class DashboardModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public string BusinessName { get; set; }

    public DashboardModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        BusinessName = user?.BusinessModel?.Name ?? "(Unknown)";
    }
}
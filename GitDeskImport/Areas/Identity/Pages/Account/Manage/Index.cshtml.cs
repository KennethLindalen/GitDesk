// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GitDeskImport.Contexts;
using GitDeskImport.Models.User;
using GitDeskImport.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GitDeskImport.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        private readonly TokenProtector _tokenProtector;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AppDbContext context,
            TokenProtector tokenProtector)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenProtector = tokenProtector;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            public string ZendeskDomain { get; set; }
            public string ZendeskApiToken { get; set; }

            public string GitHubUsername { get; set; }
            public string GitHubToken { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
{
    var user = await _context.Users
        .Include(u => u.BusinessModel)
        .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

    if (user == null)
    {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
    }

    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
    user.BusinessModel.AttachProtector(_tokenProtector);

    Input = new InputModel
    {
        Email = user.Email,
        PhoneNumber = phoneNumber,

        // Custom additions
        ZendeskDomain = user.BusinessModel.ZendeskDomain,
        ZendeskApiToken = user.BusinessModel.ZendeskApiToken,
        GitHubUsername = user.BusinessModel.GitHubUsername,
        GitHubToken = user.BusinessModel.GitHubToken
    };

    return Page();
}

public async Task<IActionResult> OnPostAsync()
{
    var user = await _context.Users
        .Include(u => u.BusinessModel)
        .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

    if (user == null)
    {
        return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
    }

    if (!ModelState.IsValid)
    {
        await OnGetAsync(); // reload input model
        return Page();
    }

    var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
    if (Input.PhoneNumber != phoneNumber)
    {
        var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
        if (!setPhoneResult.Succeeded)
        {
            StatusMessage = "Unexpected error when trying to set phone number.";
            return RedirectToPage();
        }
    }

    user.Email = Input.Email;
    user.UserName = Input.Email;

    user.BusinessModel.AttachProtector(_tokenProtector);
    user.BusinessModel.ZendeskDomain = Input.ZendeskDomain;
    user.BusinessModel.ZendeskApiToken = Input.ZendeskApiToken;
    user.BusinessModel.GitHubUsername = Input.GitHubUsername;
    user.BusinessModel.GitHubToken = Input.GitHubToken;

    await _userManager.UpdateAsync(user);
    _context.Update(user.BusinessModel);
    await _context.SaveChangesAsync();

    await _signInManager.RefreshSignInAsync(user);
    StatusMessage = "Your profile has been updated";
    return RedirectToPage();
}

    }
}

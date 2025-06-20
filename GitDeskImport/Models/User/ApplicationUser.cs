﻿namespace GitDeskImport.Models.User;

using Microsoft.AspNetCore.Identity;
using System;

public class ApplicationUser : IdentityUser
{
    public Guid BusinessId { get; set; }
    public global::BusinessModel BusinessModel { get; set; }

    public string Role { get; set; } = "Member"; // or "Admin"
}

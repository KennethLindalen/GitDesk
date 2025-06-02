using GitDeskImport.Mappers;
using GitDeskImport.Models;
using GitDeskImport.Models.Business;
using GitDeskImport.Models.User;

namespace GitDeskImport.Contexts;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<BusinessModel> Businesses { get; set; }
    public DbSet<SyncMapping> SyncMappings { get; set; }
    public DbSet<SyncConfiguration> SyncConfigurations { get; set; }
    public DbSet<BusinessInvite> BusinessInvites { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BusinessModel>()
            .Ignore(b => b.ZendeskApiToken)
            .Ignore(b => b.GitHubToken)
            .HasMany(b => b.Users)
            .WithOne(u => u.BusinessModel)
            .HasForeignKey(u => u.BusinessId);

        builder.Entity<BusinessModel>()
            .HasMany(b => b.SyncMappings)
            .WithOne(m => m.BusinessModel)
            .HasForeignKey(m => m.BusinessId);
    }
}

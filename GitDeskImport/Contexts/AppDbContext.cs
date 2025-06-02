using GitDeskImport.Mappers;
using GitDeskImport.Models.Business;
using GitDeskImport.Models.User;

namespace GitDeskImport.Contexts;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Business> Businesses { get; set; }
    public DbSet<SyncMapping> SyncMappings { get; set; }
    public DbSet<BusinessInvite> BusinessInvites { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Business>()
            .Ignore(b => b.ZendeskApiToken)
            .Ignore(b => b.GitHubToken)
            .HasMany(b => b.Users)
            .WithOne(u => u.Business)
            .HasForeignKey(u => u.BusinessId);

        builder.Entity<Business>()
            .HasMany(b => b.SyncMappings)
            .WithOne(m => m.Business)
            .HasForeignKey(m => m.BusinessId);
    }
}

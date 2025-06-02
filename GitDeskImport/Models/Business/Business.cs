using System.ComponentModel.DataAnnotations.Schema;
using GitDeskImport.Mappers;
using GitDeskImport.Models.User;
using GitDeskImport.Services;

public class Business
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    private string _zendeskApiToken;
    private string _githubToken;
    public string WebhookSecret { get; set; }

    public string ZendeskDomain { get; set; }

    [NotMapped]
    public string ZendeskApiToken
    {
        get => _tokenProtector?.Unprotect(_zendeskApiToken);
        set => _zendeskApiToken = _tokenProtector?.Protect(value);
    }

    [NotMapped]
    public string GitHubToken
    {
        get => _tokenProtector?.Unprotect(_githubToken);
        set => _githubToken = _tokenProtector?.Protect(value);
    }

    public string GitHubUsername { get; set; }

    // Raw encrypted values for EF Core to persist
    public string EncryptedZendeskApiToken
    {
        get => _zendeskApiToken;
        set => _zendeskApiToken = value;
    }

    public string EncryptedGitHubToken
    {
        get => _githubToken;
        set => _githubToken = value;
    }

    [NotMapped]
    private TokenProtector _tokenProtector;

    public void AttachProtector(TokenProtector protector)
    {
        _tokenProtector = protector;
    }

    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public ICollection<SyncMapping> SyncMappings { get; set; } = new List<SyncMapping>();
}
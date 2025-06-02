namespace GitDeskImport.Controllers.Business.DTOs;

public class RegisterBusinessRequest
{
    public string BusinessName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public string ZendeskDomain { get; set; }
    public string ZendeskApiToken { get; set; }

    public string GitHubUsername { get; set; }
    public string GitHubToken { get; set; }
}
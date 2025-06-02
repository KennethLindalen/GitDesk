namespace GitDeskImport.Models;

public class WizardState
{
    public string SourceSystem { get; set; } = string.Empty;
    public string? Repository { get; set; } // GitHub
    public string? ZendeskDomain { get; set; } // Zendesk
    public string? SyncFrequency { get; set; }
}

namespace GitDeskImport.Models;

public class SyncConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BusinessModelId { get; set; }
    public BusinessModel BusinessModel { get; set; }

    public string SourceSystem { get; set; } = string.Empty;

    // GitHub
    public string? Repository { get; set; }

    // Zendesk
    public string? ZendeskDomain { get; set; }

    public string? SyncFrequency { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
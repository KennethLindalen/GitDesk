

namespace GitDeskImport.Mappers;

using System;

public class SyncMapping
{
    public Guid Id { get; set; }

    public long ZendeskTicketId { get; set; }
    public string GitHubRepoFullName { get; set; }  // e.g., "octocat/repo"
    public int GitHubIssueNumber { get; set; }

    public DateTime LastSyncedAt { get; set; }

    public Guid BusinessId { get; set; }
    public BusinessModel BusinessModel { get; set; }
}

using GitDeskImport.Contexts;
using GitDeskImport.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace GitDeskImport.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("webhook")]
public class WebhookController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenProtector  _protector;

    public WebhookController(AppDbContext context, TokenProtector  provider)
    {
        _context = context;
        _protector = provider;
    }

    [HttpPost("github")]
    public async Task<IActionResult> GitHubWebhook([FromBody] JsonElement body)
    {
        var eventType = Request.Headers["X-GitHub-Event"].FirstOrDefault();

        if (eventType != "issues" && eventType != "issue_comment")
            return Ok(); // Ignore unrelated events

        var repo = body.GetProperty("repository").GetProperty("full_name").GetString();
        var issueNumber = body.GetProperty("issue").GetProperty("number").GetInt32();

        var mapping = await _context.SyncMappings
            .Include(m => m.BusinessModel)
            .FirstOrDefaultAsync(m => m.GitHubRepoFullName == repo && m.GitHubIssueNumber == issueNumber);

        if (mapping == null)
            return NotFound();

        mapping.BusinessModel.AttachProtector(_protector);

        var github = new GitHubService(mapping.BusinessModel.GitHubToken);
        var zendesk = new ZendeskService(mapping.BusinessModel.ZendeskApiToken, mapping.BusinessModel.ZendeskDomain);

        var issue = await github.GetIssue(repo, issueNumber);
        await zendesk.AddInternalNote(mapping.ZendeskTicketId, $"GitHub updated: {issue.Title}");

        mapping.LastSyncedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("zendesk")]
    public async Task<IActionResult> ZendeskWebhook([FromBody] JsonElement body)
    {
        var token = Request.Headers["X-Zendesk-Token"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized();

        var business = await _context.Businesses
            .FirstOrDefaultAsync(b => b.WebhookSecret == token);

        if (business == null)
            return Unauthorized();

        if (!body.TryGetProperty("ticket_id", out var ticketIdProp))
            return BadRequest();

        var ticketId = ticketIdProp.GetInt64();

        var mapping = await _context.SyncMappings
            .FirstOrDefaultAsync(m => m.ZendeskTicketId == ticketId && m.BusinessId == business.Id);

        if (mapping == null)
            return NotFound();

        business.AttachProtector(_protector);

        var zendesk = new ZendeskService(business.ZendeskApiToken, business.ZendeskDomain);
        var github = new GitHubService(business.GitHubToken);

        var ticket = await zendesk.GetTicket(ticketId);
        await github.PostComment(mapping.GitHubRepoFullName, mapping.GitHubIssueNumber, $"Zendesk updated: {ticket.Subject}");

        mapping.LastSyncedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok();
    }

}

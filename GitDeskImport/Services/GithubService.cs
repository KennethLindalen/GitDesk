namespace GitDeskImport.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

public class GitHubService
{
    private readonly HttpClient _client;

    public GitHubService(string token)
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitDeskImport", "1.0"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
    }

    public async Task<GitHubIssue> GetIssue(string repoFullName, int issueNumber)
    {
        var response = await _client.GetAsync($"https://api.github.com/repos/{repoFullName}/issues/{issueNumber}");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubIssue>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task PostComment(string repoFullName, int issueNumber, string comment)
    {
        var content = new StringContent(JsonSerializer.Serialize(new { body = comment }), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"https://api.github.com/repos/{repoFullName}/issues/{issueNumber}/comments", content);
        response.EnsureSuccessStatusCode();
    }
}

public class GitHubIssue
{
    public string Title { get; set; }
    public string Body { get; set; }
}

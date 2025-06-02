namespace GitDeskImport.Services;

using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

public class ZendeskService
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public ZendeskService(string apiToken, string domain)
    {
        _baseUrl = $"https://{domain}/api/v2/";
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"token:{apiToken}")));
    }

    public async Task<ZendeskTicket> GetTicket(long ticketId)
    {
        var response = await _client.GetAsync($"{_baseUrl}tickets/{ticketId}.json");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var ticket = doc.RootElement.GetProperty("ticket");
        return new ZendeskTicket
        {
            Subject = ticket.GetProperty("subject").GetString()
        };
    }

    public async Task AddInternalNote(long ticketId, string note)
    {
        var payload = new
        {
            ticket = new
            {
                comment = new Dictionary<string, object>
                {
                    { "body", note },
                    { "public", false } // reserved keyword workaround
                }
            }
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"{_baseUrl}tickets/{ticketId}.json", content);
        response.EnsureSuccessStatusCode();
    }

}

public class ZendeskTicket
{
    public string Subject { get; set; }
}

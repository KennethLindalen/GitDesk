namespace GitDeskImport.Models.Business;

public class BusinessInvite
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public global::Business Business { get; set; } = null!;

    public string InviteCode { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Used { get; set; } = false;
}

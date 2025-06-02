
namespace GitDeskImport.Models.Business;

public class BusinessInvite
{
    public int Id { get; set; }
    
    public Guid BusinessModelId { get; set; }
    public BusinessModel BusinessModel { get; set; }

    public string InviteCode { get; set; } = Guid.NewGuid().ToString("N");

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Used { get; set; } = false;
}

namespace Basecode.Data.Models;

public class Examination
{
    public int Id { get; set; }
    public Guid ApplicationId { get; set; }
    public int UserId { get; set; }
    public DateTime? Date { get; set; }
    public string? TeamsLink { get; set; }
    public int? Score { get; set; }
    public string? Result { get; set; }

    public Application Application { get; set; } = null!;
    public User User { get; set; } = null!;
}
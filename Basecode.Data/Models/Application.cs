namespace Basecode.Data.Models;

public class Application
{
    public Guid Id { get; set; }
    public int JobOpeningId { get; set; }
    public int ApplicantId { get; set; }
    public string Status { get; set; }
    public DateTime ApplicationDate { get; set; }
    public DateTime UpdateTime { get; set; }

    public string? Result { get; set; }
    public JobOpening JobOpening { get; set; } = null!;
    public Applicant Applicant { get; set; } = null!;
    public UserSchedule? UserSchedule { get; set; }
    public Examination? Examination { get; set; }
    public ICollection<Interview> Interviews { get; } = new List<Interview>();
}
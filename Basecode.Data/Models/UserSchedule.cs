namespace Basecode.Data.Models
{
    public class UserSchedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobOpeningId { get; set; }
        public Guid ApplicationId { get; set; }
        public string Type { get; set; }
        public DateTime Schedule { get; set; }

        public User User { get; set; }
        public JobOpening JobOpening { get; set; }
        public Application Application { get; set; }
    }
}
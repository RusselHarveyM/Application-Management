namespace Basecode.Data.Models
{
    public class UserSchedule
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string Type { get; set; }
        public DateTime Schedule { get; set; }
        public string Status { get; set; } = "pending";

        public User User { get; set; } = null!;
        public Application Application { get; set; } = null!;
    }
}
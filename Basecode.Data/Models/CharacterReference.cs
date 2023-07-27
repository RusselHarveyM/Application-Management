namespace Basecode.Data.Models
{
    public class CharacterReference
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public int ApplicantId { get; set; }
        public Applicant Applicant { get; set; } = null!;
        public BackgroundCheck? BackgroundCheck { get; set; }
    }
}

namespace Basecode.Data.ViewModels
{
    public class ConfirmedApplicantViewModel
    {
        public int ApplicantId { get; set; }
        public Guid ApplicationId { get; set; }
        public int? CurrentHireId { get; set; }
        public string FullName { get; set; }
        public string? Requirements { get; set; }
        public string Status { get; set; }
    }
}

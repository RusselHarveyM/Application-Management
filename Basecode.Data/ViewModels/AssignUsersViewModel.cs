namespace Basecode.Data.ViewModels
{
    public class AssignUsersViewModel
    {
        public int JobOpeningId { get; set; }
        public string JobOpeningTitle { get; set; }
        public List<ApplicantStatusViewModel> Applicants { get; set; }
        public List<HRUserViewModel> Users { get; set; }
    }
}

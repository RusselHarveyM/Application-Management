namespace Basecode.Data.ViewModels;

public class AssignUsersViewModel
{
    public JobOpeningBasicViewModel JobOpening { get; set; }
    public List<ApplicantStatusViewModel> Applicants { get; set; }
    public List<HRUserViewModel> Users { get; set; }
}
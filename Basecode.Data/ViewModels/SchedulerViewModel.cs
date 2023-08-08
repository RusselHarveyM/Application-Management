namespace Basecode.Data.ViewModels;

public class SchedulerViewModel
{
    public SchedulerDataViewModel FormData { get; set; }
    public List<JobOpeningBasicViewModel> JobOpenings { get; set; }
    public List<ApplicantStatusViewModel> Applicants { get; set; }
}
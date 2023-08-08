namespace Basecode.Data.ViewModels;

public class DashboardViewModel
{
    public List<JobOpeningViewModel> JobOpenings { get; set; }
    
    public int Onboarded { get; set; }
    
    public int Deployed { get; set; }
    
    public int TotalApplications { get; set; }
}
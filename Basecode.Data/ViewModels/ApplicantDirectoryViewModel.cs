using Basecode.Data.Models;

namespace Basecode.Data.ViewModels;

public class ApplicantDirectoryViewModel
{
    public List<Applicant> Applicants { get; set; }
    public ShortListedViewModel? Shortlists { get; set; }

    public List<JobOpeningViewModel>? JobOpenings { get; set; }
    public List<ApplicantExamViewModel>? ApplicantExams { get; set; }
    public ConfirmedViewModel? SignedApplicants { get; set; }
}
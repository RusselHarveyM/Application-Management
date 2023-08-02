using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationService _applicationService;
    private readonly IApplicantService _applicantService;
    private readonly IJobOpeningService _jobOpeningService;

    private readonly ITrackService _trackService;

    public DashboardService(ITrackService trackService, IApplicationService applicationService, IApplicantService applicantService, IJobOpeningService jobOpeningService)
    {
        _trackService = trackService;
        _applicationService = applicationService;
        _applicantService = applicantService;
        _jobOpeningService = jobOpeningService;
    }

    /// <summary>
    ///     Gets the shorlisted applicatons.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public List<Application> GetShorlistedApplicatons(string type, int jobId)
    {
        return _applicationService.GetShorlistedApplicatons(type, jobId);
    }

    /// <summary>
    ///     Gets the application by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Application GetApplicationById(Guid id)
    {
        return _applicationService.GetApplicationById(id);
    }

    /// <summary>
    /// Gets the directory view model for JobOpeningView view.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public ApplicantDirectoryViewModel GetApplicantDirectoryViewModel(string email, int jobId)
    {
        var applicants = _applicantService.GetApplicantsByJobOpeningIdApplicant(jobId);

        var jobs = _jobOpeningService.GetJobsWithApplications();
        foreach (var job in jobs) job.usersId = _jobOpeningService.GetLinkedUserIds(job.Id);

        var shortlistedModel = new ShortListedViewModel();
        shortlistedModel.HRShortlisted = GetShorlistedApplicatons("HR Shortlisted", jobId);
        shortlistedModel.TechShortlisted = GetShorlistedApplicatons("Technical Shortlisted", jobId);

        var applicantExams = _applicantService.GetApplicantsWithExamsByJobOpeningId(jobId);

        var directoryViewModel = new ApplicantDirectoryViewModel();
        if (email == "Admin-2-alliance@5183ny.onmicrosoft.com")
            directoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                Shortlists = shortlistedModel,
                JobOpenings = jobs,
                ApplicantExams = applicantExams,
            };
        else
            directoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                Shortlists = shortlistedModel,
                JobOpenings = jobs,
                ApplicantExams = applicantExams,
            };

        return directoryViewModel;
    }
    
    
    /// <summary>
    /// Gets the directory view model for DirectoryView view.
    /// </summary>
    /// <param name="userAspId"></param>
    /// <returns></returns>
    public async Task<ApplicantDirectoryViewModel> GetDirectoryViewModel(string email, string userAspId)
    {
        var applicants = _applicantService.GetApplicantsWithJobAndReferences(userAspId);


        var jobs = _jobOpeningService.GetJobsWithApplications();

        // Sort the job openings by updatedTime in descending order
        jobs.Sort((job1, job2) => DateTime.Compare((DateTime)job2.UpdatedTime, (DateTime)job1.UpdatedTime));

        foreach (var job in jobs) job.usersId = _jobOpeningService.GetLinkedUserIds(job.Id);

        var applicantDirectoryViewModel = new ApplicantDirectoryViewModel();
        if (email == "Admin-2-alliance@5183ny.onmicrosoft.com")
        {
            applicantDirectoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                JobOpenings = jobs
            };
        }
        else
        {
            var newJobs = new List<JobOpeningViewModel>();
            foreach (var job in jobs)
                if (job.usersId.Contains(userAspId))
                    newJobs.Add(job);
            applicantDirectoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                JobOpenings = newJobs
            };
        }
        return applicantDirectoryViewModel;
    }

    /// <summary>
    ///     Updates the status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="status">The status.</param>
    public void UpdateStatus(Application application, User user, string status)
    {
        var result = _trackService.UpdateApplicationStatus(application, user, status, null);
        if (result != null)
        {
            _applicationService.Update(result);
            _trackService.UpdateTrackStatusEmail(application, user, status, "Approval");
        }
    }
}
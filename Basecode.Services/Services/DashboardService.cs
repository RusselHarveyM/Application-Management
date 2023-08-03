using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Hangfire;

namespace Basecode.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationService _applicationService;
    private readonly IApplicantService _applicantService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly ICurrentHireService _currentHireService;

    private readonly IUserService _userService;
    private readonly ReferenceToPdf _referenceToPdf;
    private readonly ITrackService _trackService;
    private readonly IBackgroundCheckService _backgroundCheckService;
    private readonly ICharacterReferenceService _characterReferenceService;
    private readonly IEmailSendingService _emailSendingService;

    public DashboardService(ITrackService trackService, IApplicationService applicationService, IApplicantService applicantService, IJobOpeningService jobOpeningService,
        ReferenceToPdf referenceToPdf,
        IBackgroundCheckService backgroundCheckService, IUserService userService,
        ICharacterReferenceService characterReferenceService, IEmailSendingService emailSendingService, ICurrentHireService currentHireService)
    {
        _trackService = trackService;
        _applicationService = applicationService;
        _applicantService = applicantService;
        _jobOpeningService = jobOpeningService;
        _referenceToPdf = referenceToPdf;
        _backgroundCheckService = backgroundCheckService;
        _userService = userService;
        _characterReferenceService = characterReferenceService;
        _emailSendingService = emailSendingService;
        _currentHireService = currentHireService;
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

    public List<CurrentHire> GetShortListedCurrentHire(string type)
    {
        return _currentHireService.GetShortListedCurrentHire(type);
    }

    /// <summary>
    /// Get Dashboard View Model
    /// </summary>
    /// <returns></returns>
    public DashboardViewModel GetDashboardViewModel()
    {
        var dashboardview = new DashboardViewModel
        {
            JobOpenings = _jobOpeningService.GetJobsWithApplicationsSorted(),
            Onboarded = _applicationService.GetOnboarded(),
            Deployed = _applicationService.GetDeployed(),
            TotalApplications = _applicationService.GetTotalApplications()
        };

        return dashboardview;
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
        var applicants = _applicantService.GetApplicantsByJobOpeningId(jobId);

        var jobs = _jobOpeningService.GetJobsWithApplications();
        foreach (var job in jobs) job.usersId = _jobOpeningService.GetLinkedUserIds(job.Id);

        var shortlistedModel = new ShortListedViewModel();
        shortlistedModel.HRShortlisted = GetShorlistedApplicatons("HR Shortlisted", jobId);
        shortlistedModel.TechShortlisted = GetShorlistedApplicatons("Technical Shortlisted", jobId);

        var confirmedViewModel = new ConfirmedViewModel();
        confirmedViewModel.ConfirmedApplicants = _applicantService.GetConfirmedApplicants(jobId, "Confirmed");
        confirmedViewModel.OnboardingApplicants = _applicantService.GetConfirmedApplicants(jobId, "Onboarding");

        var applicantExams = _applicantService.GetApplicantsWithExamsByJobOpeningId(jobId);

        var directoryViewModel = new ApplicantDirectoryViewModel();
        if (email == "Admin-2-alliance@5183ny.onmicrosoft.com")
            directoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                Shortlists = shortlistedModel,
                JobOpenings = jobs,
                ApplicantExams = applicantExams,
                SignedApplicants = confirmedViewModel,
            };
        else
            directoryViewModel = new ApplicantDirectoryViewModel
            {
                Applicants = applicants,
                Shortlists = shortlistedModel,
                JobOpenings = jobs,
                ApplicantExams = applicantExams,
                SignedApplicants = confirmedViewModel,
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

        ApplicantDirectoryViewModel applicantDirectoryViewModel;
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
                if (job.usersId != null && job.usersId.Contains(userAspId))
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
    public void UpdateStatus(Application application, User user, string status, string mailType)
    {
        var result = _trackService.UpdateApplicationStatus(application, user, status, null);
        if (result != null)
        {
            _applicationService.Update(result);
            _trackService.UpdateTrackStatusEmail(application, user, status, mailType);
        }
    }

    public void SendListEmail(int appId, string email, string name)
    {
        var references = _characterReferenceService.GetReferencesByApplicantId(appId);
        var noReplyReferences = new List<CharacterReference>();
        foreach (var reference in references)
        {
            var backgroundCheck = _backgroundCheckService.GetBackgroundByCharacterRefId(reference.Id);
            if (backgroundCheck == null)
            {
                noReplyReferences.Add(reference);
            }
        }

        if (noReplyReferences.Count > 0)
        {
            BackgroundJob.Enqueue(() =>
                _emailSendingService.SendReferenceListReminder(email, name, noReplyReferences,
                    "A list of References who did not answer."));
            BackgroundJob.Schedule(() => ExportReferenceToPdf(email, name, appId), TimeSpan.FromHours(12));
        }
        else
        {
            BackgroundJob.Enqueue(() => ExportReferenceToPdf(email, name, appId));
        }
    }

    /// <summary>
    /// Exports references' answers to pdf
    /// </summary>
    /// <param name="appId"></param>
    public void ExportReferenceToPdf(string email, string name, int appId)
    {
        var pdfs = new List<byte[]>();
        var references = _characterReferenceService.GetReferencesByApplicantId(appId);
        foreach (var reference in references)
        {
            var backgroundCheck = _backgroundCheckService.GetBackgroundByCharacterRefId(reference.Id);
            if (backgroundCheck != null)
            {
                byte[] pdf = _referenceToPdf.ExportToPdf(backgroundCheck);
                pdfs.Add(pdf);
            }
        }

        var user = _userService.GetByEmail(email);
        var applicant = _applicantService.GetApplicantById(appId);
        var application = _applicationService.GetApplicationByApplicantId(applicant.Id);

        _emailSendingService.SendReferenceAnswers(user, applicant, application.Id,
            "Undergoing Background Check", pdfs);
    }
}
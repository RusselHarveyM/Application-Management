using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class ShortlistingService : ErrorHandling, IShortlistingService
{
    private readonly IApplicationService _applicationService;
    private readonly IExaminationService _examinationService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly ITrackService _trackService;
    private readonly IUserService _userService;

    public ShortlistingService(IExaminationService examinationService, IApplicationService applicationService,
        IJobOpeningService jobOpeningService, IUserService userService, ITrackService trackService)
    {
        _examinationService = examinationService;
        _applicationService = applicationService;
        _jobOpeningService = jobOpeningService;
        _userService = userService;
        _trackService = trackService;
    }

    /// <summary>
    ///     Shortlists the applications.
    /// </summary>
    public void ShortlistApplications()
    {
        var jobOpeningIds = _jobOpeningService.GetAllJobOpeningIds();

        foreach (var jobOpeningId in jobOpeningIds)
        {
            var exams = _examinationService.GetShortlistableExamsByJobOpeningId(jobOpeningId);
            var totalExams = exams.Count;

            if (exams == null || totalExams == 0) continue;

            var shortlistPercentage = GetShortlistPercentage(totalExams);
            var numberToShortlist = (int)Math.Ceiling(totalExams * shortlistPercentage);
            numberToShortlist = Math.Max(numberToShortlist, 1);

            var shortlistedExams = exams.OrderByDescending(e => e.Score)
                .Take(numberToShortlist)
                .ToList();
            UpdateShortlistedApplications(shortlistedExams);

            var nonShortlistedExams = exams.Except(shortlistedExams).ToList();
            if (nonShortlistedExams != null)
                UpdateNonShortlistedApplications(nonShortlistedExams);
        }
    }

    /// <summary>
    ///     Gets the shortlist percentage.
    /// </summary>
    /// <param name="totalExams">The total exams.</param>
    /// <returns></returns>
    public double GetShortlistPercentage(int totalExams)
    {
        if (totalExams <= 10) return 0.50;
        if (totalExams <= 50) return 0.30;
        return 0.20;
    }

    /// <summary>
    /// Updates the status of the shortlisted applications.
    /// </summary>
    /// <param name="examination">The examination.</param>
    public void UpdateShortlistedApplications(List<Examination> shortlistedExams)
    {
        foreach (var examination in shortlistedExams)
        {
            var application = _applicationService.GetApplicationById(examination.ApplicationId);
            var user = _userService.GetById(examination.UserId);

            application = _trackService.UpdateApplicationStatus(application, user, "Technical Shortlisted", string.Empty);
            if (application != null)
            {
                _applicationService.Update(application);
            }
        }
    }

    /// <summary>
    /// Updates the non shortlisted applications.
    /// </summary>
    /// <param name="nonShortlistedExams">The non shortlisted exams.</param>
    public void UpdateNonShortlistedApplications(List<Examination> nonShortlistedExams)
    {
        foreach (var examination in nonShortlistedExams)
        {
            var application = _applicationService.GetApplicationById(examination.ApplicationId);
            var user = _userService.GetById(examination.UserId);

            application = _trackService.UpdateApplicationStatus(application, user, "Rejected", "Rejected");
            if (application != null)
            {
                _applicationService.Update(application);
            }
        }
    }
}
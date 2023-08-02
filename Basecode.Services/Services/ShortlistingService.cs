using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class ShortlistingService : ErrorHandling, IShortlistingService
{
    private readonly IApplicationService _applicationService;
    private readonly IExaminationService _examinationService;
    private readonly IJobOpeningService _jobOpeningService;

    public ShortlistingService(IExaminationService examinationService, IApplicationService applicationService,
        IJobOpeningService jobOpeningService)
    {
        _examinationService = examinationService;
        _applicationService = applicationService;
        _jobOpeningService = jobOpeningService;
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

            var applicationIds = shortlistedExams.Select(e => e.ApplicationId).ToList();
            var applications = _applicationService.GetApplicationsByIds(applicationIds);

            foreach (var application in applications)
            {
                application.Status = "Technical Shortlisted";
                _applicationService.Update(application);
            }
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
}
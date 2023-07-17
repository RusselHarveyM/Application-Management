using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class ShortlistingService : ErrorHandling, IShortlistingService
    {
        private readonly IExaminationService _examinationService;
        private readonly IApplicationService _applicationService;
        private readonly IJobOpeningService _jobOpeningService;

        public ShortlistingService(IExaminationService examinationService, IApplicationService applicationService, IJobOpeningService jobOpeningService)
        {
            _examinationService = examinationService;
            _applicationService = applicationService;
            _jobOpeningService = jobOpeningService;
        }

        /// <summary>
        /// Shortlists the applications.
        /// </summary>
        public void ShortlistApplications()
        {
            List<int> jobOpeningIds = _jobOpeningService.GetAllJobOpeningIds();

            foreach (int jobOpeningId in jobOpeningIds)
            {
                var exams = _examinationService.GetExaminationsByJobOpeningId(jobOpeningId);
                int totalExams = exams.Count;

                if (exams == null || totalExams == 0) continue;

                double shortlistPercentage = GetShortlistPercentage(totalExams);
                int numberToShortlist = (int)Math.Ceiling(totalExams * shortlistPercentage);
                numberToShortlist = Math.Max(numberToShortlist, 1);

                List<Examination> shortlistedExams = exams.OrderByDescending(e => e.Score)
                                                  .Take(numberToShortlist)
                                                  .ToList();

                var applicationIds = shortlistedExams.Select(e => e.ApplicationId).ToList();
                var applications = _applicationService.GetApplicationsByIds(applicationIds);

                foreach (var application in applications)
                {
                    application.Status = "Shortlisted";
                    _applicationService.Update(application);
                }
            }
        }

        /// <summary>
        /// Gets the shortlist percentage.
        /// </summary>
        /// <param name="totalExams">The total exams.</param>
        /// <returns></returns>
        public double GetShortlistPercentage(int totalExams)
        {
            if (totalExams <= 10) return 0.50;
            else if (totalExams <= 50) return 0.30;
            else return 0.20;
        }

    }
}

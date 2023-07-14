using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class ExaminationService : ErrorHandling, IExaminationService
    {
        private readonly IExaminationRepository _repository;
        private readonly IApplicationService _applicationService;

        public ExaminationService(IExaminationRepository repository , IApplicationService applicationService)
        {
            _repository = repository;
            _applicationService = applicationService;
        }

        public List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId)
        {
            return _repository.GetExaminationsByJobOpeningId(jobOpeningId).ToList();
        }

        public List<Examination> ShortlistExaminations(int jobOpeningId)
        {

            //var exams = GetExaminationsByJobOpeningId(jobOpeningId);

            // Temporary hardcode until the creation of Examinations will be sorted out
            var exams = new List<Examination>()
                {
                    new Examination
                    {
                        Id = 1,
                        UserId_HR = 1,
                        Date = new DateOnly(2023,1,1),
                        TeamsLink = "link.test",
                        Score = 79,
                        Result = "For Technical Exam",
                        ApplicationId = Guid.NewGuid()
                    },
                    new Examination
                    {
                        Id = 2,
                        UserId_HR = 2,
                        Date = new DateOnly(2023,1,1),
                        TeamsLink = "link.test",
                        Score = 75,
                        Result = "For Technical Exam",
                        ApplicationId = Guid.NewGuid()
                    },
                    new Examination
                    {
                        Id = 3,
                        UserId_HR = 3,
                        Date = new DateOnly(2023,1,1),
                        TeamsLink = "link.test",
                        Score = 95,
                        Result = "For Technical Exam",
                        ApplicationId = Guid.NewGuid()
                    },
                    new Examination
                    {
                        Id = 4,
                        UserId_HR = 4,
                        Date = new DateOnly(2023,1,1),
                        TeamsLink = "link.test",
                        Score = 89,
                        Result = "For Technical Exam",
                        ApplicationId = Guid.NewGuid()
                    },
                };

            int totalExams = exams.Count;
            if (exams == null || totalExams == 0)
            {
                throw new Exception("No examinations found.");
            }

            double shortlistPercentage = GetShortlistPercentage(totalExams);
            int numberToShortlist = (int)Math.Ceiling(totalExams * shortlistPercentage);
            numberToShortlist = Math.Max(numberToShortlist, 1);

            List<Examination> shortlistedExams = exams.OrderByDescending(e => e.Score)
                                              .Take(numberToShortlist)
                                              .ToList();

            // Uncomment the code below when the creation of Examinations is sorted out
            //var applicationIds = shortlistedExams.Select(e => e.ApplicationId).ToList();
            //var applications = _applicationService.GetApplicationsByIds(applicationIds);

            //foreach (var application in applications)
            //{
            //    application.Status = "Shortlisted";
            //    _applicationService.Update(application);
            //}

            return shortlistedExams;
        }

        public double GetShortlistPercentage(int totalExams)
        {
            if (totalExams <= 10) return 0.50;
            else if (totalExams <= 50) return 0.30;
            else return 0.20;
        }
    }
}

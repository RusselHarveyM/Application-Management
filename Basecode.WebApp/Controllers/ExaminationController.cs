using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly IExaminationService _service;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ExaminationController(IExaminationService service)
        {
            _service = service;
        }

        public List<Examination> Shortlist()
        {
            List<Examination> exams = new List<Examination>()
            { 
                new Examination
                {
                    Id = 1,
                    UserId_HR = 1,
                    Date = new DateOnly(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 79,
                    Result = "PASS",
                    ApplicationId = 1
                },
                new Examination
                {
                    Id = 2,
                    UserId_HR = 2,
                    Date = new DateOnly(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 75,
                    Result = "PASS",
                    ApplicationId = 2
                },
                new Examination
                {
                    Id = 3,
                    UserId_HR = 3,
                    Date = new DateOnly(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 95,
                    Result = "PASS",
                    ApplicationId = 3
                },
                new Examination
                {
                    Id = 4,
                    UserId_HR = 4,
                    Date = new DateOnly(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 89,
                    Result = "PASS",
                    ApplicationId = 4
                },
            };

            exams = exams.OrderByDescending(a => a.Score).ToList();
            List<Examination> shortlistedExams = exams.Take(2).ToList();
            return shortlistedExams;
        }
    }
}

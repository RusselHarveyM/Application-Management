using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Basecode.Tests.Controllers
{
    public class ExaminationControllerTests
    {
        private readonly Mock<IExaminationService> _fakeExaminationService;
        private readonly ExaminationController _controller;

        public ExaminationControllerTests()
        {
            _fakeExaminationService = new Mock<IExaminationService>();
            _controller = new ExaminationController(_fakeExaminationService.Object);
        }

        [Fact]
        public void Shortlist_HasExaminations_ReturnsShortlist()
        {
            // Arrange
            List<Examination> expectedExams = new List<Examination>()
            {
                new Examination
                {
                    Id = 3,
                    UserId_HR = 3,
                    Date = new DateTime(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 95,
                    Result = "For Technical Exam",
                    ApplicationId = Guid.NewGuid(),
                },
                new Examination
                {
                    Id = 4,
                    UserId_HR = 4,
                    Date = new DateTime(2023,1,1),
                    TeamsLink = "link.test",
                    Score = 89,
                    Result = "For Technical Exam",
                    ApplicationId = Guid.NewGuid(),
                },
            };
            int jobOpeningId = 1;
            _fakeExaminationService.Setup(service => service.ShortlistExaminations(jobOpeningId)).Returns(expectedExams);

            // Act
            var result = _controller.Shortlist(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var shortlistedExams = Assert.IsType<List<Examination>>(okResult.Value);
            for (int i = 0; i < expectedExams.Count; i++)
            {
                Assert.Equal(expectedExams[i].Id, shortlistedExams[i].Id);
                Assert.Equal(expectedExams[i].UserId_HR, shortlistedExams[i].UserId_HR);
                Assert.Equal(expectedExams[i].Score, shortlistedExams[i].Score);
            }
        }

        [Fact]
        public void Shortlist_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            List<Examination> expectedNoExams = new List<Examination>();
            int jobOpeningId = 1;
            _fakeExaminationService.Setup(service => service.ShortlistExaminations(jobOpeningId)).Throws(new Exception());

            // Act
            var result = _controller.Shortlist(1);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }
    }
}

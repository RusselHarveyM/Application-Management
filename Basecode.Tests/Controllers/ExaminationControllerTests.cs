using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
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

            // Act
            var result = _controller.Shortlist();

            // Assert
            Assert.IsType<List<Examination>>(result);
            Assert.Equal(2, result.Count);
            for (int i = 0; i < expectedExams.Count; i++)
            {
                Assert.Equal(expectedExams[i].Id, result[i].Id);
                Assert.Equal(expectedExams[i].UserId_HR, result[i].UserId_HR);
                Assert.Equal(expectedExams[i].Score, result[i].Score);
            }
        }

    }
}

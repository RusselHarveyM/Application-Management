using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Basecode.Tests.Controllers
{
    public class TrackerControllerTestsface
    {
        private readonly Mock<IApplicationService> _fakeApplicationService;
        private readonly Mock<IUserService> _fakeUserService;
        private readonly Mock<ITrackService> _fakeTrackService;
        private readonly TrackerController _controller;
        private readonly Mock<IConfiguration> _fakeConfig;

        public TrackerControllerTestsface()
        {
            _fakeApplicationService = new Mock<IApplicationService>();
            _fakeTrackService = new Mock<ITrackService>();
            _fakeUserService = new Mock<IUserService>();
            _fakeConfig = new Mock<IConfiguration>();
            _fakeConfig.Setup(x => x["TokenHelper:SecretKey"]).Returns("fakeSecretKey");
            _controller = new TrackerController(_fakeApplicationService.Object, _fakeUserService.Object, _fakeTrackService.Object, _fakeConfig.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void ResultView_ExistingId_ReturnsViewResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var application = new ApplicationViewModel { Id = id };
            _fakeApplicationService.Setup(service => service.GetById(id)).Returns(application);

            // Act
            var result = _controller.ResultView(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(application, viewResult.Model);
        }

        [Fact]
        public void ResultView_NotExistingId_ReturnsViewResult()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            var application = (ApplicationViewModel)null;
            _fakeApplicationService.Setup(service => service.GetById(id)).Returns(application);

            // Act
            var result = _controller.ResultView(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(application, viewResult.Model);

            Assert.True(_controller.ViewData.ContainsKey("ErrorMessage"));
            var errorMessage = _controller.ViewData["ErrorMessage"] as string;
            Assert.Equal("Application not found.", errorMessage);
        }

        [Fact]
        public void ResultView_Exception_ReturnsServerError()
        {
            // Arrange
            Guid id = Guid.NewGuid();
            _fakeApplicationService.Setup(service => service.GetById(id)).Throws(new Exception());

            // Act
            var result = _controller.ResultView(id);

            // Assert
            var viewResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}

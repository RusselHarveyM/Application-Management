using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Moq;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Tests.Controllers
{
    public class BackgroundCheckControllerTests
    {
        private readonly BackgroundCheckController _controller;
        private readonly Mock<ICharacterReferenceService> _fakeCharacterReferenceService;
        private readonly Mock<IApplicantService> _fakeApplicantService;
        private readonly Mock<IApplicationService> _fakeApplicationService;
        private readonly Mock<IJobOpeningService> _fakeJobOpeningService;
        private readonly Mock<IBackgroundCheckService> _fakeBackgroundCheckService;
        private readonly Mock<IEmailSendingService> _fakeEmailSendingService;
        private readonly Mock<IUserService> _fakeUserService;
        private readonly Mock<IConfiguration> _fakeConfig;
        private readonly Mock<TokenHelper> _fakeTokenHelper;

        public BackgroundCheckControllerTests()
        {
            _fakeCharacterReferenceService = new Mock<ICharacterReferenceService>();
            _fakeApplicantService = new Mock<IApplicantService>();
            _fakeApplicationService = new Mock<IApplicationService>();
            _fakeJobOpeningService = new Mock<IJobOpeningService>();
            _fakeBackgroundCheckService = new Mock<IBackgroundCheckService>();
            _fakeEmailSendingService = new Mock<IEmailSendingService>();
            _fakeUserService = new Mock<IUserService>();
            _fakeConfig = new Mock<IConfiguration>();
            _fakeConfig.Setup(x => x["TokenHelper:SecretKey"]).Returns("fakeSecretKey");

            var fakeToastNotification = new Mock<IToastNotification>();
            _controller = new BackgroundCheckController(
                _fakeCharacterReferenceService.Object,
                _fakeApplicantService.Object,
                _fakeApplicationService.Object,
                _fakeJobOpeningService.Object,
                _fakeBackgroundCheckService.Object,
                _fakeEmailSendingService.Object,
                _fakeUserService.Object,
                _fakeConfig.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object,
                ControllerContext = new ControllerContext()
            };
        }

        [Fact]
        public void Index_WithInvalidToken_ReturnsRedirectToActionResult()
        {
            // Arrange
            var token = "invalidToken";

            // Act
            var result = _controller.Index(token);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task FormOk_WithValidData_ReturnsViewResult()
        {
            // Arrange
            var formData = new BackgroundCheckFormViewModel();
            _fakeBackgroundCheckService.Setup(service => service.Create(It.IsAny<BackgroundCheckFormViewModel>()));

            // Act
            var result = await _controller.FormOk(formData);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Redirection", viewResult.ViewName);
        }

        [Fact]
        public async Task FormOk_WithInvalidData_ReturnsServerError()
        {
            // Arrange
            var formData = new BackgroundCheckFormViewModel();
            _fakeBackgroundCheckService.Setup(service => service.Create(It.IsAny<BackgroundCheckFormViewModel>()))
                .Throws(new Exception());

            // Act
            var result = await _controller.FormOk(formData);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, ((ObjectResult)result).StatusCode);
        }
    }
}

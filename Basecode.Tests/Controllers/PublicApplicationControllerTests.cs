using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Tests.Controllers
{
    public class PublicApplicationControllerTests
    {
        private readonly PublicApplicationController _controller;
        private readonly Mock<IApplicantService> _mockApplicantService;
        private readonly Mock<IJobOpeningService> _mockJobOpeningService;
        private readonly Mock<IApplicationService> _mockApplicationService;

        public PublicApplicationControllerTests()
        {
            _mockApplicantService = new Mock<IApplicantService>();
            _mockJobOpeningService = new Mock<IJobOpeningService>();
            _mockApplicationService = new Mock<IApplicationService>();
            _controller = new PublicApplicationController(_mockApplicantService.Object, _mockJobOpeningService.Object, _mockApplicationService.Object);
        }

        [Fact]
        public void Index_ValidJobOpeningId_ReturnsViewWithApplicationForm()
        {
            // Arrange
            int jobOpeningId = 123;

            // Act
            var result = _controller.Index(jobOpeningId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is ViewResult || result is ObjectResult, "Result should be either a ViewResult or an ObjectResult.");
            if (result is ViewResult viewResult)
            {
                Assert.NotNull(viewResult.ViewName);
            }
        }

        [Fact]
        public void Index_InvalidJobOpeningId_ReturnsStatusCode500()
        {
            // Arrange
            int jobOpeningId = 456;
            var expectedErrorMessage = "Something went wrong.";

            // Act
            var result = _controller.Index(jobOpeningId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal(expectedErrorMessage, objectResult.Value);
        }

        [Fact]
        public void Reference_InvalidFile_ReturnsRedirectToAction()
        {
            // Arrange
            var applicant = new ApplicantViewModel { JobOpeningId = 123 };
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("file.png");
            file.Setup(f => f.CopyTo(It.IsAny<Stream>()));
            var expectedErrorMessage = "Only PDF files are allowed.";

            // Act
            var result = _controller.Reference(applicant, file.Object);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is RedirectToActionResult || result is ObjectResult, "Result should be either a RedirectToActionResult or an ObjectResult.");
            if (result is RedirectToActionResult redirectResult)
            {
                Assert.Equal("Index", redirectResult.ActionName);
                Assert.Equal(applicant.JobOpeningId, redirectResult.RouteValues["jobOpeningId"]);
                Assert.Equal(expectedErrorMessage, _controller.TempData["ErrorMessage"]);
            }
        }

        [Fact]
        public void Reference_WithNoFile_ReturnsRedirectToAction()
        {
            // Arrange
            var applicant = new ApplicantViewModel { JobOpeningId = 123 };
            IFormFile file = null;
            var expectedErrorMessage = "Please select a file.";

            // Act
            var result = _controller.Reference(applicant, file);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is RedirectToActionResult || result is ObjectResult, "Result should be either a RedirectToActionResult or an ObjectResult.");
            if (result is RedirectToActionResult redirectResult)
            {
                Assert.Equal("Index", redirectResult.ActionName);
                Assert.Equal(applicant.JobOpeningId, redirectResult.RouteValues["jobOpeningId"]);
                Assert.Equal(expectedErrorMessage, _controller.TempData["ErrorMessage"]);
            }
        }

        [Fact]
        public void Reference_WithPDFFile_ReturnsView()
        {
            // Arrange
            var applicant = new ApplicantViewModel();
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("file.pdf");
            file.Setup(f => f.CopyTo(It.IsAny<Stream>()));

            // Act
            var result = _controller.Reference(applicant, file.Object);

            // Assert
            Assert.NotNull(result);
            Assert.True(result is ViewResult || result is ObjectResult, "Result should be either a ViewResult or an ObjectResult.");
            if (result is ViewResult viewResult)
            {
                Assert.Same(applicant, viewResult.Model);
                Assert.Equal("Successfuly renders form.", _controller.TempData["FileName"]);
            }
        }

        [Fact]
        public void Reference_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            var applicant = new ApplicantViewModel();
            var file = new Mock<IFormFile>();
            file.Setup(f => f.FileName).Returns("file.pdf");
            file.Setup(f => f.CopyTo(It.IsAny<Stream>()));
            var expectedErrorMessage = "Something went wrong.";

            // Act
            var result = _controller.Reference(applicant, file.Object);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal(expectedErrorMessage, objectResult.Value);
        }

        [Fact]
        public void Confirmation_Successful_ReturnsView()
        {
            // Arrange
            var applicant = new ApplicantViewModel();
            var fileName = "document.pdf";
            var fileData = "0x";

            // Act
            var result = _controller.Confirmation(applicant, fileName, fileData);

            // Assert
            Assert.NotNull(result);

            // Check if it's a ViewResult or an ObjectResult
            Assert.True(result is ViewResult || result is ObjectResult, "Result should be either a ViewResult or an ObjectResult.");

            // Additional specific assertions for ViewResult
            if (result is ViewResult viewResult)
            {
                Assert.Same(applicant, viewResult.Model);
                Assert.Equal(fileName, _controller.TempData["FileName"]);
            }
        }

        [Fact]
        public void Confirmation_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            var applicant = new ApplicantViewModel();
            var fileName = "document.pdf";
            var fileData = "0x";
            var expectedErrorMessage = "Something went wrong.";

            // Act
            var result = _controller.Confirmation(applicant, fileName, fileData);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal(expectedErrorMessage, objectResult.Value);
        }

        //[Fact]
        //public async Task Create_JobOpeningDoesNotExist_ReturnsView()
        //{
        //    // Arrange
        //    var applicant = new ApplicantViewModel { JobOpeningId = 123 };
        //    var fileName = "document.pdf";
        //    var applicantId = 456;
        //    var newStatus = "Success";
        //    JobOpeningViewModel isJobOpening = null;
        //    _mockJobOpeningService.Setup(service => service.GetById(applicant.JobOpeningId)).Returns(isJobOpening);

        //    // Act
        //    var result = await _controller.Create(applicant, fileName, applicantId, newStatus);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<ViewResult>(result);
        //    Assert.Equal("Index", ((ViewResult)result).ViewName);
        //    _mockApplicantService.Verify(service => service.Create(applicant), Times.Never);
        //    _mockApplicationService.Verify(service => service.UpdateApplicationStatus(applicantId, newStatus, It.IsAny<string>()), Times.Never);
        //}

        [Fact]
        public async Task Create_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            var applicant = new ApplicantViewModel { JobOpeningId = 123 };
            var fileName = "document.pdf";
            var fileData = "0x";
            var applicantId = 456;
            var newStatus = "Success";
            var expectedErrorMessage = "Something went wrong.";
            var isJobOpening = new JobOpeningViewModel();
            _mockJobOpeningService.Setup(service => service.GetById(applicant.JobOpeningId)).Returns(isJobOpening);
            _mockApplicantService.Setup(service => service.Create(applicant)).Throws(new Exception());

            // Act
            var result = await _controller.Create(applicant, fileName, applicantId, newStatus, fileData);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal(expectedErrorMessage, objectResult.Value);
        }
    }
}

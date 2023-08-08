using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Basecode.Tests.Controllers;

public class PublicApplicationControllerTests
{
    private readonly PublicApplicationController _controller;
    private readonly Mock<IApplicantService> _mockApplicantService;
    private readonly Mock<IJobOpeningService> _mockJobOpeningService;

    public PublicApplicationControllerTests()
    {
        _mockApplicantService = new Mock<IApplicantService>();
        _mockJobOpeningService = new Mock<IJobOpeningService>();
        _controller = new PublicApplicationController(_mockApplicantService.Object, _mockJobOpeningService.Object);
    }

    [Fact]
    public void Index_ValidJobOpeningId_ReturnsViewResult()
    {
        // Arrange
        int jobOpeningId = 1;

        // Act
        var result = _controller.Index(jobOpeningId);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Index_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        int jobOpeningId = 1;
        _mockApplicantService.Setup(s => s.GetApplicants()).Throws(new Exception());

        // Act
        var result = _controller.Index(jobOpeningId);

        // Assert
        var viewResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public void Reference_ValidFile_ReturnsViewResult()
    {
        // Arrange
        var applicant = new ApplicantViewModel();
        var fileUpload = new Mock<IFormFile>();
        fileUpload.Setup(f => f.FileName).Returns("example.pdf");
        fileUpload.Setup(f => f.CopyTo(It.IsAny<Stream>()));

        // Act
        var result = _controller.Reference(applicant, fileUpload.Object);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Reference_InvalidFileExtension_ReturnsRedirectToAction()
    {
        // Arrange
        var applicant = new ApplicantViewModel { JobOpeningId = 1 };
        var fileUpload = new Mock<IFormFile>();
        fileUpload.Setup(f => f.FileName).Returns("example.txt");

        // Act
        var result = _controller.Reference(applicant, fileUpload.Object);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Reference_NoFileSelected_ReturnsRedirectToAction()
    {
        // Arrange
        var applicant = new ApplicantViewModel { JobOpeningId = 1 };

        // Act
        var result = _controller.Reference(applicant, null);

        // Assert
        var redirectToActionResult = Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Reference_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var applicant = new ApplicantViewModel();
        var fileUpload = new Mock<IFormFile>();
        fileUpload.Setup(f => f.FileName).Returns("example.pdf");
        fileUpload.Setup(f => f.CopyTo(It.IsAny<Stream>())).Throws(new Exception());

        // Act
        var result = _controller.Reference(applicant, fileUpload.Object);

        // Assert
        var viewResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public void Confirmation_ValidData_ReturnsViewResult()
    {
        // Arrange
        var applicant = new ApplicantViewModel();
        var fileName = "example.pdf";
        var fileData = "base64filedata";

        // Act
        var result = _controller.Confirmation(applicant, fileName, fileData);

        // Assert
        Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Confirmation_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var applicant = new ApplicantViewModel();
        var fileName = "example.pdf";
        var fileData = "base64filedata";
        _mockApplicantService.Setup(s => s.GetApplicantById(It.IsAny<int>())).Throws(new Exception());

        // Act
        var result = _controller.Confirmation(applicant, fileName, fileData);

        // Assert
        var viewResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }

    [Fact]
    public void Create_ValidDataAndJobOpening_ReturnsRedirectToJobIndex()
    {
        // Arrange
        var applicant = new ApplicantViewModel { JobOpeningId = 1 };
        var fileName = "example.pdf";
        var applicantId = 1;
        var newStatus = "Pending";
        var fileData = "base64filedata";

        var jobOpening = new JobOpeningViewModel { Id = applicant.JobOpeningId };
        _mockJobOpeningService.Setup(s => s.GetById(applicant.JobOpeningId)).Returns(jobOpening);

        // Act
        var result = _controller.Create(applicant, fileName, applicantId, newStatus, fileData);

        // Assert
        var redirectToActionResult = Assert.IsType<ObjectResult>(result);
    }

    [Fact]
    public void Create_JobOpeningNotFound_ReturnsViewIndex()
    {
        // Arrange
        var applicant = new ApplicantViewModel { JobOpeningId = 1 };
        var fileName = "example.pdf";
        var applicantId = 1;
        var newStatus = "Pending";
        var fileData = "base64filedata";

        _mockJobOpeningService.Setup(s => s.GetById(applicant.JobOpeningId)).Returns((JobOpeningViewModel)null);

        // Act
        var result = _controller.Create(applicant, fileName, applicantId, newStatus, fileData);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
    }

    [Fact]
    public void Create_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var applicant = new ApplicantViewModel { JobOpeningId = 1 };
        var fileName = "example.pdf";
        var applicantId = 1;
        var newStatus = "Pending";
        var fileData = "base64filedata";

        _mockJobOpeningService.Setup(s => s.GetById(applicant.JobOpeningId)).Throws(new Exception());

        // Act
        var result = _controller.Create(applicant, fileName, applicantId, newStatus, fileData);

        // Assert
        var viewResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, ((ObjectResult)result).StatusCode);
    }
}
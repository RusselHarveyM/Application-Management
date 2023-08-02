using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NToastNotify;

namespace Basecode.Tests.Controllers;

public class JobControllerTests
{
    private readonly JobController _controller;
    private readonly Mock<IJobOpeningService> _fakeJobOpeningService;

    public JobControllerTests()
    {
        _fakeJobOpeningService = new Mock<IJobOpeningService>();
        Mock<UserManager<IdentityUser>> fakeUserManager = new(Mock.Of<IUserStore<IdentityUser>>(), null, null, null,
            null, null, null, null, null);
        Mock<IToastNotification> fakeToastNotification = new();

        _controller = new JobController(_fakeJobOpeningService.Object, fakeUserManager.Object,
            fakeToastNotification.Object);
    }


    [Fact]
    public void Index_HasJobs_ReturnsJobs()
    {
        // Arrange
        var expectedJobs = new List<JobOpeningViewModel>
        {
            new()
            {
                Id = 1,
                Title = "Dummy Job Opening",
                EmploymentType = "Full-time",
                WorkSetup = "Remote",
                Location = "New York",
                Qualifications = new List<Qualification>
                {
                    new() { Id = 1, Description = "Bachelor's degree" },
                    new() { Id = 2, Description = "2+ years of experience" }
                },
                Responsibilities = new List<Responsibility>
                {
                    new() { Id = 1, Description = "Develop and maintain software applications" },
                    new() { Id = 2, Description = "Collaborate with cross-functional teams" }
                }
            }
        };

        _fakeJobOpeningService.Setup(service => service.GetJobs()).Returns(expectedJobs);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var actualJobs = Assert.IsAssignableFrom<List<JobOpeningViewModel>>(viewResult.Model);

        Assert.Equal(expectedJobs, actualJobs);
        Assert.NotEmpty(actualJobs);
    }

    [Fact]
    public void Index_HasNoJobs_ReturnsEmpty()
    {
        // Arrange
        var expectedNoJobs = new List<JobOpeningViewModel>();
        _fakeJobOpeningService.Setup(service => service.GetJobs()).Returns(expectedNoJobs);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var actualJobs = Assert.IsAssignableFrom<List<JobOpeningViewModel>>(viewResult.Model);

        Assert.Equal(expectedNoJobs, actualJobs);
        Assert.Empty(actualJobs);
    }

    [Fact]
    public void Index_Exception_ReturnsServerError()
    {
        // Arrange
        _fakeJobOpeningService.Setup(service => service.GetJobs()).Throws(new Exception());

        // Act
        var result = _controller.Index();

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void CreateView_HasEmptyJob_ReturnsServiceException()
    {
        // Arrange

        // Act
        var result = _controller.CreateView();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<JobOpeningViewModel>(viewResult.ViewData.Model);
    }

    [Fact]
    public void JobView_ExistingId_ReturnsViewResult()
    {
        // Arrange
        var id = 1;
        var jobOpening = new JobOpeningViewModel { Id = id };
        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns(jobOpening);

        // Act
        var result = _controller.JobView(id);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(jobOpening, viewResult.Model);
    }

    [Fact]
    public void JobView_NonExistingId_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 1;
        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns((JobOpeningViewModel)null);

        // Act
        var result = _controller.JobView(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void JobView_Exception_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 1;
        _fakeJobOpeningService.Setup(service => service.GetById(id)).Throws(new Exception());

        // Act
        var result = _controller.JobView(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }


    [Fact]
    public async Task Create_Exception_ReturnsRedirectToActionResult()
    {
        // Arrange
        var jobOpening = new JobOpeningViewModel();
        _fakeJobOpeningService.Setup(service => service.Create(jobOpening, It.IsAny<string>())).Throws(new Exception());

        // Act
        var result = await _controller.Create(jobOpening);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }


    [Fact]
    public void UpdateView_ExistingId_ReturnsViewResult()
    {
        // Arrange
        var id = 1;

        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns(new JobOpeningViewModel());


        // Act
        var result = _controller.UpdateView(id);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void UpdateView_NonExistingId_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 2;

        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns((JobOpeningViewModel)null);


        // Act
        var result = _controller.UpdateView(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void UpdateView_ExceptionThrown_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 3;

        _fakeJobOpeningService.Setup(service => service.GetById(id)).Throws(new Exception());


        // Act
        var result = _controller.UpdateView(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    //[Fact]
    //public void Update_ValidJobOpening_RedirectsToIndex()
    //{
    //    // Arrange
    //    var jobOpening = new JobOpeningViewModel
    //    {
    //        Id = 1,
    //    };
    //    string updatedBy = "dummy1";

    //    var logContent = new LogContent { Result = false }; 
    //    _fakeJobOpeningService.Setup(service => service.Update(jobOpening, updatedBy)).Returns(logContent);

    //    // Act
    //    var result = _controller.Update(jobOpening);

    //    // Assert
    //    var redirectResult = Assert.IsType<RedirectToActionResult>(result);
    //    Assert.Equal("Index", redirectResult.ActionName);
    //}

    //[Fact]

    //public void Update_ValidationFailed_ReturnsUpdateView()
    //{
    //    // Arrange
    //    var jobOpening = new JobOpeningViewModel
    //    {
    //        Id = 1,
    //    };
    //    string updatedBy = "dummy1";

    //    var logContent = new LogContent { Result = true }; 
    //    _fakeJobOpeningService.Setup(service => service.Update(jobOpening, updatedBy)).Returns(logContent);


    //    // Act
    //    var result = _controller.Update(jobOpening);

    //    // Assert
    //    var viewResult = Assert.IsType<ViewResult>(result);
    //    Assert.Equal("UpdateView", viewResult.ViewName);
    //    Assert.Equal(jobOpening, viewResult.Model);
    //}

    [Fact]
    public void Update_ExceptionThrown_ReturnsRedirectToActionResult()
    {
        // Arrange
        var jobOpening = new JobOpeningViewModel
        {
            Id = 1
        };
        var updatedBy = "dummy1";

        _fakeJobOpeningService.Setup(service => service.Update(jobOpening, updatedBy)).Throws(new Exception());


        // Act
        var result = _controller.Update(jobOpening);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void Delete_ExistingId_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 1;

        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns(new JobOpeningViewModel { Id = id });

        // Act
        var result = _controller.Delete(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public void Delete_NonExistingId_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 2;

        _fakeJobOpeningService.Setup(service => service.GetById(id)).Returns((JobOpeningViewModel)null);

        // Act
        var result = _controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Delete_ExceptionThrown_ReturnsRedirectToActionResult()
    {
        // Arrange
        var id = 3;
        // Mock the behavior of _jobOpeningService.GetById(id) to throw an exception
        _fakeJobOpeningService.Setup(service => service.GetById(id)).Throws(new Exception());

        // Act
        var result = _controller.Delete(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}
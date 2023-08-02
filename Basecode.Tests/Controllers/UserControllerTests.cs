using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Tests.Controllers;

public class UserControllerTests
{
    private readonly UserController _controller;
    private readonly Mock<IUserService> _fakeUserService;

    public UserControllerTests()
    {
        _fakeUserService = new Mock<IUserService>();
        _controller = new UserController(_fakeUserService.Object, null, null);
    }

    [Fact]
    public void Index_HasUsers_ReturnsUsers()
    {
        // Arrange
        var expectedUsers = new List<UserViewModel>
        {
            new()
        };
        _fakeUserService.Setup(service => service.RetrieveAll()).Returns(expectedUsers);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var actualUsers = Assert.IsAssignableFrom<List<UserViewModel>>(viewResult.Model);
        Assert.Equal(expectedUsers, actualUsers);
        Assert.NotEmpty(actualUsers);
    }

    [Fact]
    public void Index_HasNoUsers_ReturnsEmpty()
    {
        // Arrange
        var expectedNoUsers = new List<UserViewModel>();
        _fakeUserService.Setup(service => service.RetrieveAll()).Returns(expectedNoUsers);

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var actualUsers = Assert.IsAssignableFrom<List<UserViewModel>>(viewResult.Model);
        Assert.Equal(expectedNoUsers, actualUsers);
        Assert.Empty(actualUsers);
    }

    [Fact]
    public void Index_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        _fakeUserService.Setup(service => service.RetrieveAll()).Throws(new Exception());

        // Act
        var result = _controller.Index();

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public void AddView_HasUserModel_ReturnsPartialViewResult()
    {
        var result = _controller.AddView();

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("~/Views/User/_AddView.cshtml", partialViewResult.ViewName);
    }

    [Fact]
    public async Task Create_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("User", "The Username field is required.");

        // Act
        var result = await _controller.Create(new UserViewModel());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_ValidModelState_ReturnsOkResult()
    {
        // Arrange
        var user = new UserViewModel();
        _fakeUserService.Setup(service => service.Create(user)).ReturnsAsync(new LogContent());

        // Act
        var result = await _controller.Create(user);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Create_ServiceValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var logContent = new LogContent();
        logContent.Result = true;
        logContent.ErrorCode = "400";
        logContent.Message = "The Email Address format is invalid.";
        var user = new UserViewModel();
        _fakeUserService.Setup(service => service.Create(user)).ReturnsAsync(logContent);

        // Act
        var result = await _controller.Create(user);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<JsonResult>(badRequestResult.Value);
    }

    [Fact]
    public async Task Create_FailedToGetErrors_ReturnsStatusCode500()
    {
        // Arrange
        _fakeUserService.Setup(service => service.GetValidationErrors(_controller.ModelState))
            .Throws(new Exception());

        // Act
        var result = await _controller.Create(new UserViewModel());

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task UpdateView_NonexistentId_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 420;
        User? data = null;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(data);

        // Act
        var result = await _controller.UpdateView(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateView_ExistingId_ReturnsPartialViewResult()
    {
        // Arrange
        var id = 69;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(new User());

        // Act
        var result = await _controller.UpdateView(id);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("~/Views/User/_UpdateView.cshtml", partialViewResult.ViewName);
    }

    [Fact]
    public async Task UpdateView_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var id = 25;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.UpdateView(id);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Update_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("User", "The Username field is required.");

        // Act
        var result = await _controller.Update(new UserUpdateViewModel());

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<SerializableError>(badRequestResult.Value);
    }

    [Fact]
    public async Task Update_ValidModelState_ReturnsOkResult()
    {
        // Arrange
        var user = new UserUpdateViewModel();
        _fakeUserService.Setup(service => service.Update(user)).ReturnsAsync(new LogContent());

        // Act
        var result = await _controller.Update(user);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Update_ServiceValidationFailed_ReturnsBadRequest()
    {
        // Arrange
        var logContent = new LogContent();
        logContent.Result = true;
        logContent.ErrorCode = "400";
        logContent.Message = "The Email Address format is invalid.";
        var user = new UserUpdateViewModel();
        _fakeUserService.Setup(service => service.Update(user)).ReturnsAsync(logContent);

        // Act
        var result = await _controller.Update(user);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.IsType<JsonResult>(badRequestResult.Value);
    }

    [Fact]
    public async Task Update_FailedToGetErrors_ReturnsStatusCode500()
    {
        // Arrange
        _fakeUserService.Setup(service => service.GetValidationErrors(_controller.ModelState))
            .Throws(new Exception());

        // Act
        var result = await _controller.Update(new UserUpdateViewModel());

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task DeleteView_NonexistentId_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 420;
        User? data = null;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(data);

        // Act
        var result = await _controller.DeleteView(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteView_ExistingId_ReturnsPartialViewResult()
    {
        // Arrange
        var id = 69;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(new User());

        // Act
        var result = await _controller.DeleteView(id);

        // Assert
        var partialViewResult = Assert.IsType<PartialViewResult>(result);
        Assert.Equal("~/Views/User/_DeleteView.cshtml", partialViewResult.ViewName);
    }

    [Fact]
    public async Task DeleteView_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var id = 25;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.DeleteView(id);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task Delete_NonexistentId_ReturnsNotFoundResult()
    {
        // Arrange
        var id = 420;
        User? data = null;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(data);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingId_RedirectsToIndex()
    {
        // Arrange
        var id = 69;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ReturnsAsync(new User());

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Delete_ExceptionThrown_ReturnsStatusCode500()
    {
        // Arrange
        var id = 25;
        _fakeUserService.Setup(service => service.GetByIdAsync(id)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
    }
}
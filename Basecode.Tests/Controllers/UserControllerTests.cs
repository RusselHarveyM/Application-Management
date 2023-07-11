﻿using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _fakeUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _fakeUserService = new Mock<IUserService>();
            _controller = new UserController(_fakeUserService.Object);
        }

        [Fact]
        public void Index_HasUsers_ReturnsUsers()
        {
            // Arrange
            var expectedUsers = new List<UserViewModel>
            {
                new UserViewModel()
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
        public void Create_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("User", "The Username field is required.");

            // Act
            var result = _controller.Create(new User());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void Create_ValidModelState_ReturnsOkResult()
        {
            // Arrange
            var user = new User();
            _fakeUserService.Setup(service => service.Create(user)).Returns(new LogContent());

            // Act
            var result = _controller.Create(user);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Create_ServiceValidationFailed_ReturnsBadRequest()
        {
            // Arrange
            LogContent logContent = new LogContent();
            logContent.Result = true;
            logContent.ErrorCode = "400";
            logContent.Message = "Email address does not have a domain.";
            var user = new User();
            _fakeUserService.Setup(service => service.Create(user)).Returns(logContent);

            // Act
            var result = _controller.Create(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<JsonResult>(badRequestResult.Value);
        }

        [Fact]
        public void Create_FailedToGetErrors_ReturnsStatusCode500()
        {
            // Arrange
            _fakeUserService.Setup(service => service.GetValidationErrors(_controller.ModelState))
                .Throws(new Exception());

            // Act
            var result = _controller.Create(new User());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void UpdateView_NonexistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 420;
            User? data = null;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(data);

            // Act
            var result = _controller.UpdateView(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateView_ExistingId_ReturnsPartialViewResult()
        {
            // Arrange
            int id = 69;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(new User());

            // Act
            var result = _controller.UpdateView(id);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("~/Views/User/_UpdateView.cshtml", partialViewResult.ViewName);
        }

        [Fact]
        public void UpdateView_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            int id = 25;
            _fakeUserService.Setup(service => service.GetById(id)).Throws(new Exception());

            // Act
            var result = _controller.UpdateView(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void Update_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("User", "The Username field is required.");

            // Act
            var result = _controller.Update(new User());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public void Update_ValidModelState_ReturnsOkResult()
        {
            // Arrange
            var user = new User();
            _fakeUserService.Setup(service => service.Update(user)).Returns(new LogContent());

            // Act
            var result = _controller.Update(user);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void Update_ServiceValidationFailed_ReturnsBadRequest()
        {
            // Arrange
            LogContent logContent = new LogContent();
            logContent.Result = true;
            logContent.ErrorCode = "400";
            logContent.Message = "Email address does not have a domain.";
            var user = new User();
            _fakeUserService.Setup(service => service.Update(user)).Returns(logContent);

            // Act
            var result = _controller.Update(user);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<JsonResult>(badRequestResult.Value);
        }

        [Fact]
        public void Update_FailedToGetErrors_ReturnsStatusCode500()
        {
            // Arrange
            _fakeUserService.Setup(service => service.GetValidationErrors(_controller.ModelState))
                .Throws(new Exception());

            // Act
            var result = _controller.Update(new User());

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void DeleteView_NonexistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 420;
            User? data = null;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(data);

            // Act
            var result = _controller.DeleteView(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteView_ExistingId_ReturnsPartialViewResult()
        {
            // Arrange
            int id = 69;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(new User());

            // Act
            var result = _controller.DeleteView(id);

            // Assert
            var partialViewResult = Assert.IsType<PartialViewResult>(result);
            Assert.Equal("~/Views/User/_DeleteView.cshtml", partialViewResult.ViewName);
        }

        [Fact]
        public void DeleteView_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            int id = 25;
            _fakeUserService.Setup(service => service.GetById(id)).Throws(new Exception());

            // Act
            var result = _controller.DeleteView(id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void Delete_NonexistentId_ReturnsNotFoundResult()
        {
            // Arrange
            int id = 420;
            User? data = null;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(data);

            // Act
            var result = _controller.Delete(id);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_ExistingId_RedirectsToIndex()
        {
            // Arrange
            int id = 69;
            _fakeUserService.Setup(service => service.GetById(id)).Returns(new User());

            // Act
            var result = _controller.Delete(id);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public void Delete_ExceptionThrown_ReturnsStatusCode500()
        {
            // Arrange
            int id = 25;
            _fakeUserService.Setup(service => service.GetById(id)).Throws(new Exception());

            // Act
            var result = _controller.Delete(id);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}

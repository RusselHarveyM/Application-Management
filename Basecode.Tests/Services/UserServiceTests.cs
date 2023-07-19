using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly UserService _service;
        private readonly Mock<IJobOpeningService> _fakeJobOpeningService;

        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeJobOpeningService = new Mock<IJobOpeningService>();
            _service = new UserService(_fakeUserRepository.Object, _fakeJobOpeningService.Object);
        }

        [Fact]
        public void RetrieveAll_HasUsers_ReturnsUserViewModelList()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, Fullname = "John Doe", Username = "doe.john", Email = "john@example.com", Role = "Human Resources" },
                new User { Id = 2, Fullname = "Jane Smith", Username = "smith_jane", Email = "jane@example.com", Role = "Deployment Team" }
            };
            _fakeUserRepository.Setup(repository => repository.RetrieveAll()).Returns(expectedUsers.AsQueryable());

            // Act
            var result = _service.RetrieveAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserViewModel>>(result);
            Assert.Equal(2, result.Count);

            for (int i = 0; i < expectedUsers.Count; i++)
            {
                Assert.Equal(expectedUsers[i].Id, result[i].Id);
                Assert.Equal(expectedUsers[i].Fullname, result[i].Fullname);
                Assert.Equal(expectedUsers[i].Username, result[i].Username);
                Assert.Equal(expectedUsers[i].Email, result[i].Email);
                Assert.Equal(expectedUsers[i].Role, result[i].Role);
            }
        }

        [Fact]
        public void Create_ValidationFailed_ReturnsLogContent()
        {
            // Arrange
            var user = new User { Email = "invalid@email" };

            // Act
            var result = _service.Create(user);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LogContent>(result);
            Assert.True(result.Result);
            Assert.Equal("400", result.ErrorCode);
        }

        [Fact]
        public void Create_ValidUserModel_ReturnsLogContent()
        {
            // Arrange
            var expectedUser = new User()
            {
                Fullname = "John Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "8password",
                Role = "Human Resources"
            };
            User? actualUser = null;
            _fakeUserRepository.Setup(repository => repository.Create(It.IsAny<User>()))
                .Callback<User>(u => actualUser = u);

            // Act
            var result = _service.Create(expectedUser);
            _fakeUserRepository.Verify(repository => repository.Create(It.IsAny<User>()), Times.Once);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LogContent>(result);
            Assert.False(result.Result);
            Assert.Equal("", result.ErrorCode);
            Assert.Equal("", result.Message);

            Assert.Equal(expectedUser.Fullname, actualUser.Fullname);
            Assert.Equal(expectedUser.Username, actualUser.Username);
            Assert.Equal(expectedUser.Email, actualUser.Email);
            Assert.Equal(expectedUser.Password, actualUser.Password);
            Assert.Equal(expectedUser.Role, actualUser.Role);
        }

        [Fact]
        public void GetById_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { Id = userId, Fullname = "John Doe", Email = "johndoe@example.com" };
            _fakeUserRepository.Setup(repository => repository.GetById(userId)).Returns(expectedUser);

            // Act
            var result = _service.GetById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<User>(result);
            Assert.Equal(expectedUser.Id, result.Id);
            Assert.Equal(expectedUser.Fullname, result.Fullname);
            Assert.Equal(expectedUser.Email, result.Email);
        }

        [Fact]
        public void GetById_NonexistentId_ReturnsNull()
        {
            // Arrange
            var userId = 100;
            _fakeUserRepository.Setup(r => r.GetById(userId)).Returns((User)null);

            // Act
            var result = _service.GetById(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Update_ValidationFailed_ReturnsLogContent()
        {
            // Arrange
            var user = new User { Email = "invalid@email" };

            // Act
            var result = _service.Update(user);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LogContent>(result);
            Assert.True(result.Result);
            Assert.Equal("400", result.ErrorCode);
        }

        [Fact]
        public void Update_ValidUserModel_ReturnsLogContent()
        {
            // Arrange
            var user = new User()
            {
                Id = 1,
                Fullname = "John Doe",
                Username = "doe.john",
                Email = "john@example.com",
                Password = "8password",
                Role = "Human Resources"
            };
            _fakeUserRepository.Setup(repository => repository.GetById(user.Id)).Returns(user);

            // Act
            var result = _service.Update(user);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<LogContent>(result);
            Assert.False(result.Result);
            Assert.Equal("", result.ErrorCode);
            Assert.Equal("", result.Message);
        }

        [Fact]
        public void Delete_WithUser_CallsDeleteOnce()
        {
            // Arrange
            var user = new User();

            // Act
            _service.Delete(user);

            // Assert
            _fakeUserRepository.Verify(r => r.Delete(user), Times.Once);
        }

        [Fact]
        public void GetValidationErrors_HasErrors_ReturnsDictionary()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Email", "Email address must be valid");
            modelState.AddModelError("Password", "Password must be at least 8 characters");

            // Act
            var result = _service.GetValidationErrors(modelState);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Dictionary<string, string>>(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Email address must be valid", result["Email"]);
            Assert.Equal("Password must be at least 8 characters", result["Password"]);
        }
    }
}
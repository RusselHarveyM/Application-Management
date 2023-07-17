//using Basecode.Data.Models;
//using Basecode.Data.ViewModels;
//using Basecode.Main.Controllers;
//using Basecode.Services.Interfaces;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Basecode.Tests.Controllers
//{
//    public class HomeControllerTests
//    {
//        private readonly Mock<IJobOpeningService> _fakeJobOpeningService;
//        private readonly HomeController _controller;

//        public HomeControllerTests()
//        {
//            _fakeJobOpeningService = new Mock<IJobOpeningService>();
//            _controller = new HomeController(_fakeJobOpeningService.Object);
//        }

//        [Fact]
//        public void Index_HasJobs_ReturnsJobs()
//        {
//            // Arrange
//            var expectedJobs = new List<JobOpeningViewModel>
//            {
//                new JobOpeningViewModel
//                {
//                    Id = 1,
//                    Title = "Software Developer",
//                    EmploymentType = "Contructual",
//                    WorkSetup = "Hybrid",
//                    Location = "Cebu City, Cebu",
//                    Qualifications = new List<Qualification>
//                    {
//                        new Qualification { Id = 1, Description = "Bachelor's Degree" },
//                        new Qualification { Id = 2, Description = "3+ years of experience" }
//                    },
//                    Responsibilities = new List<Responsibility>
//                    {
//                        new Responsibility { Id = 1, Description = "Develop and Maintain Software Applications"},
//                        new Responsibility { Id = 2, Description = "Can work under pressure"}
//                    }
//                }
//            };

//            _fakeJobOpeningService.Setup(service => service.GetJobs()).Returns(expectedJobs);

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var actualJobs = Assert.IsAssignableFrom<List<JobOpeningViewModel>>(viewResult.Model);

//            Assert.Equal(expectedJobs, actualJobs);
//            Assert.NotEmpty(actualJobs);
//        }

//        [Fact]
//        public void Index_HasNoJobs_ReturnsEmpty()
//        {
//            // Arrange
//            var expectedNoJobs = new List<JobOpeningViewModel>();
//            _fakeJobOpeningService.Setup(service => service.GetJobs()).Returns(expectedNoJobs);

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var actualJobs = Assert.IsAssignableFrom<List<JobOpeningViewModel>>(viewResult.Model);

//            Assert.Equal(expectedNoJobs, actualJobs);
//            Assert.Empty(actualJobs);
//        }

//        [Fact]
//        public void Index_Exception_ReturnsServerError()
//        {
//            // Arrange
//            _fakeJobOpeningService.Setup(service => service.GetJobs()).Throws(new Exception());

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(500, ((ObjectResult)result).StatusCode);
//        }
//    }
//}

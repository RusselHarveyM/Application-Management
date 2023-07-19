//using Basecode.Data.Models;
//using Basecode.Services.Interfaces;
//using Basecode.WebApp.Controllers;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Basecode.Tests.Controllers
//{
//    public class DashboardControllerTests
//    {
//        private readonly Mock<IApplicantService> _fakeApplicantService;
//        private readonly DashboardController _controller;

//        public DashboardControllerTests() 
//        {
//            _fakeApplicantService = new Mock<IApplicantService>();
//            _controller = new DashboardController(_fakeApplicantService.Object);
//        }

//        [Fact]
//        public void Index_HasApplicants_ReturnsViewWithModel()
//        {
//            // Arrange
//            var applicants = new List<Applicant>
//            {
//                new Applicant { Id = 1, Firstname = "John", Lastname = "Doe" },
//                new Applicant { Id = 2, Firstname = "Jane", Lastname = "Smith" }
//            };

//            _fakeApplicantService.Setup(service => service.GetApplicants()).Returns(applicants);

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = Assert.IsAssignableFrom<List<Applicant>>(viewResult.Model);
//            Assert.Equal(applicants, model);
//        }

//        [Fact]
//        public void Index_HasNoApplicants_ReturnsEmpty()
//        {
//            // Arrange
//            var applicants = new List<Applicant>();

//            _fakeApplicantService.Setup(service => service.GetApplicants()).Returns(applicants);

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var viewResult = Assert.IsType<ViewResult>(result);
//            var model = Assert.IsAssignableFrom<List<Applicant>>(viewResult.Model);
//            Assert.Empty(model);
//        }

//        [Fact]
//        public void Index_Exception_ReturnsServerError()
//        {
//            // Arrange
//            _fakeApplicantService.Setup(service => service.GetApplicants()).Throws(new Exception());

//            // Act
//            var result = _controller.Index();

//            // Assert
//            var statusCodeResult = Assert.IsType<ObjectResult>(result);
//            Assert.Equal(500, statusCodeResult.StatusCode);
//        }
//    }
//}

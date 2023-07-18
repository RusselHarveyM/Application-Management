using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Basecode.Tests.Controllers
{
    public class ExaminationControllerTests
    {
        private readonly Mock<IExaminationService> _fakeExaminationService;
        private readonly ExaminationController _controller;

        public ExaminationControllerTests()
        {
            _fakeExaminationService = new Mock<IExaminationService>();
            _controller = new ExaminationController(_fakeExaminationService.Object);
        }

    }
}

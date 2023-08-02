using Basecode.Services.Interfaces;
using Basecode.WebApp.Controllers;
using Moq;

namespace Basecode.Tests.Controllers;

public class ExaminationControllerTests
{
    private readonly ExaminationController _controller;
    private readonly Mock<IExaminationService> _fakeExaminationService;

    public ExaminationControllerTests()
    {
        _fakeExaminationService = new Mock<IExaminationService>();
        _controller = new ExaminationController(_fakeExaminationService.Object);
    }
}
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers;

public class ExaminationController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IExaminationService _service;

    public ExaminationController(IExaminationService service)
    {
        _service = service;
    }
}
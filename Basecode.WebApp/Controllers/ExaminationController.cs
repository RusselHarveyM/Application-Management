using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class ExaminationController : Controller
    {
        private readonly IExaminationService _service;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ExaminationController(IExaminationService service)
        {
            _service = service;
        }

        public IActionResult Shortlist(int jobOpeningId)
        {
            try
            {
                List<Examination> shortlistedExams = _service.ShortlistExaminations(jobOpeningId);
                return Ok(shortlistedExams);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

    }
}

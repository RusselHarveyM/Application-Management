using Basecode.Services.Interfaces;
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

        public void Shortlist()
        {

        }
    }
}

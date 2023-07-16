using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class TrackerController : Controller
    {
        private readonly IApplicationService _applicationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackerController"/> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public TrackerController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>A view of the tracker page</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Retrieves an application with the given Id
        /// </summary>
        /// <param name="id">The application ID.</param>
        /// <returns>A view with either the tracker result table or an error message</returns>
        [HttpGet]
        public IActionResult ResultView(Guid id)
        {
            try
            {
                var application = _applicationService.GetById(id);
                if (application == null)
                {
                    ViewData["ErrorMessage"] = "Application not found.";
                    _logger.Error("Application [" + id + "] not found!");
                    return View("Index");
                }
                else
                {
                    _logger.Trace("Application [" + id + "] found.");
                    return View("Index", application);
                }
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        [HttpPost]
        public IActionResult ChangeStatus(string status)
        {
            // Perform the necessary logic to update the applicant's status in your data store
            // using the provided ID and status value.

            // Redirect the user back to the original page or display a success message.
            return RedirectToAction("ChangeStatusView");
        }


        public IActionResult ChangeStatusView()
        {
            return View();
        }
    }
}
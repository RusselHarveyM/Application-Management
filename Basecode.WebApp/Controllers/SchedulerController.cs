using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly IUserScheduleService _userScheduleService;
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenHelper _tokenHelper;
        private const string SecretKey = "CDC1CAAACAA3269755F5EC44C7202F0055C9C322AEB5C4B6103F6E9C11EF136F";
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SchedulerController(IUserScheduleService userScheduleService, IUserService userService, IApplicantService applicantService, UserManager<IdentityUser> userManager)
        {
            _userScheduleService = userScheduleService;
            _userService = userService;
            _applicantService = applicantService;
            _userManager = userManager;
            _tokenHelper = new TokenHelper(SecretKey);
        }

        /// <summary>
        /// Displays the HR Scheduler.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                var userAspId = _userManager.GetUserId(User);
                var jobOpenings = _userService.GetLinkedJobOpenings(userAspId);
                var applicants = _applicantService.GetApplicantsWithRejectedOrNoSchedule();
                var schedulerFormData = new SchedulerDataViewModel();

                SchedulerViewModel viewModel = new SchedulerViewModel()
                {
                    FormData = schedulerFormData,
                    JobOpenings = jobOpenings,
                    Applicants = applicants,
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Creates records in the UserSchedule table.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SchedulerDataViewModel formData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warn("Model has validation error(s).");
                    return View(formData);
                }

                await _userScheduleService.AddUserSchedules(formData);
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        [Route("Scheduler/AcceptSchedule/{token}")]
        public IActionResult AcceptSchedule(string token)
        {
            try
            {
                ViewBag.IsScheduleAccepted = false;
                int userScheduleId = _tokenHelper.GetIdFromToken(token, "accept");
                if (userScheduleId == 0)
                {
                    _logger.Warn("Invalid or expired token.");
                    return View();
                }

                var data = _userScheduleService.AcceptSchedule(userScheduleId);
                if (!data.Result)
                {
                    _logger.Trace("User Schedule [" + userScheduleId + "] has been successfully accepted.");
                    ViewBag.IsScheduleAccepted = true;
                }

                return View();
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        [Route("Scheduler/RejectSchedule/{token}")]
        public async Task<IActionResult> RejectSchedule(string token)
        {
            try
            {
                ViewBag.IsScheduleRejected = false;
                int userScheduleId = _tokenHelper.GetIdFromToken(token, "reject");
                if (userScheduleId == 0)
                {
                    _logger.Warn("Invalid or expired token.");
                    return View();
                }

                var data = await _userScheduleService.RejectSchedule(userScheduleId);
                if (!data.Result)
                {
                    _logger.Trace("User Schedule [" + userScheduleId + "] has been successfully rejected.");
                    ViewBag.IsScheduleRejected = true;
                }

                return View();
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            } 
        }
    }
}

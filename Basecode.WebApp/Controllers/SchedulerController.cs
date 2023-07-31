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
        private readonly ISchedulerService _schedulerService;
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly UserManager<IdentityUser> _userManager;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly TokenHelper _tokenHelper;

        public SchedulerController(IUserService userService, IApplicantService applicantService, UserManager<IdentityUser> userManager, IConfiguration config, ISchedulerService schedulerService)
        {
            _userService = userService;
            _applicantService = applicantService;
            _userManager = userManager;
            _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
            _schedulerService = schedulerService;
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
        public IActionResult Create(SchedulerDataViewModel formData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warn("Model has validation error(s).");
                    return BadRequest();
                }
                var userAspId = _userManager.GetUserId(User);
                int userId = _userService.GetUserIdByAspId(userAspId);
                (ErrorHandling.LogContent logContent, Dictionary<string, string> validationErrors) data = _schedulerService.AddSchedules(formData, userId);

                if (!data.logContent.Result)
                {
                    _logger.Trace("Successfully added new user schedule(s).");
                    return Ok();
                }

                _logger.Error(ErrorHandling.SetLog(data.logContent));
                return BadRequest(Json(data.validationErrors));
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
        public async Task<IActionResult> AcceptSchedule(string token)
        {
            try
            {
                int userScheduleId = _tokenHelper.GetIdFromToken(token, "accept");
                if (userScheduleId == 0)
                {
                    ViewBag.IsInvalidToken = true;
                    _logger.Warn("Invalid or expired token.");
                    return View();
                }

                ViewBag.IsSuccessfullyAccepted = false;
                var data = await _schedulerService.AcceptSchedule(userScheduleId);
                if (!data.Result)
                {
                    _logger.Trace("User Schedule [" + userScheduleId + "] has been successfully accepted.");
                    ViewBag.IsSuccessfullyAccepted = true;
                }
                else _logger.Error(ErrorHandling.SetLog(data));

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
        public IActionResult RejectSchedule(string token)
        {
            try
            {
                int userScheduleId = _tokenHelper.GetIdFromToken(token, "reject");
                if (userScheduleId == 0)
                {
                    ViewBag.IsInvalidToken = true;
                    _logger.Warn("Invalid or expired token.");
                    return View();
                }

                ViewBag.IsSuccessfullyRejected = false;
                var data = _schedulerService.RejectSchedule(userScheduleId);
                if (!data.Result)
                {
                    _logger.Trace("User Schedule [" + userScheduleId + "] has been successfully rejected.");
                    ViewBag.IsSuccessfullyRejected = true;
                }
                else _logger.Error(ErrorHandling.SetLog(data));

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

﻿using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly IUserScheduleService _userScheduleService;
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly IExaminationService _examinationService;
        private readonly IInterviewService _interviewService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SignInManager<IdentityUser> _signInManager;

        public SchedulerController(IUserScheduleService userScheduleService, IUserService userService, SignInManager<IdentityUser> signInManager, 
            IApplicantService applicantService, IInterviewService interviewService, IExaminationService examinationService)
        {
            _userScheduleService = userScheduleService;
            _userService = userService;
            _applicantService = applicantService;
            _signInManager = signInManager;
            _interviewService = interviewService;
            _examinationService = examinationService;
        }

        /// <summary>
        /// Displays the HR Scheduler.
        /// </summary>
        [HttpGet]
        public IActionResult CreateView()
        {
            try
            {
                int userId = 1;     // temporary until User auth is sorted out
                var jobOpenings = _userService.GetLinkedJobOpenings(userId);
                var applicants = _applicantService.GetApplicantsWithStatuses();
                var userScheduleViewModel = new SchedulerDataViewModel();
                SchedulerViewModel viewModel = new SchedulerViewModel()
                {
                    FormData = userScheduleViewModel,
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
                    _logger.Warn("Invalid ModelState.");
                    return BadRequest(ModelState);
                }

                await _userScheduleService.AddUserSchedules(formData);

                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        public IActionResult AcceptSchedule()
        {
            try
            {
                int userScheduleId;
                string userScheduleIdString = HttpContext.Request.Query["userScheduleId"];
                if (!userScheduleIdString.IsNullOrEmpty())
                {
                    userScheduleId = Int32.Parse(userScheduleIdString);
                }
                else 
                {
                    _logger.Warn("Invalid query string");
                    return NotFound();
                }

                var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);

                if (userSchedule == null)
                {
                    _logger.Error("Schedule is not found");
                    return NotFound();
                }

                if (userSchedule.Status == "accepted")
                {
                    _logger.Error("Schedule has already been accepted");
                    return Unauthorized();
                }

                if (userSchedule.Type == "For HR Interview" || userSchedule.Type == "For Technical Interview")
                {
                    var interviewData = _interviewService.AddInterview(userSchedule);
                    if (!interviewData.Result)
                    {
                        _logger.Trace("Successfully created a new Interview record.");
                    }
                }
                else
                {
                    var examData = _examinationService.AddExamination(userSchedule);
                    if (!examData.Result)
                    {
                        _logger.Trace("Successfully created a new Examination record.");
                    }
                }

                userSchedule.Status = "accepted";
                var scheduleData = _userScheduleService.UpdateUserSchedule(userSchedule);
                if (!scheduleData.Result)
                {
                    _logger.Trace("User schedule [" + userScheduleId + "] has been successfully accepted.");
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
        /// <returns></returns>
        public IActionResult RejectSchedule()
        {
            try
            {
                int userScheduleId;
                string userScheduleIdString = HttpContext.Request.Query["userScheduleId"];
                if (!userScheduleIdString.IsNullOrEmpty())
                {
                    userScheduleId = Int32.Parse(userScheduleIdString);
                }
                else
                {
                    _logger.Warn("Invalid query string");
                    return NotFound();
                }

                var userSchedule = _userScheduleService.GetUserScheduleById(userScheduleId);

                if (userSchedule == null)
                {
                    return NotFound();
                }

                if (userSchedule.Status == "rejected")
                {

                }

                // Perform the rejection logic here
                // Update the status of the UserSchedule to "rejected"
                userSchedule.Status = "rejected";
                _userScheduleService.UpdateUserSchedule(userSchedule);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            } 
        }
    }
}

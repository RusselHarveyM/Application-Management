using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;

namespace Basecode.WebApp.Controllers
{
    public class SchedulerController : Controller
    {
        private readonly IUserScheduleService _userScheduleService;
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly IApplicationService _applicationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SignInManager<IdentityUser> _signInManager;

        public SchedulerController(IUserScheduleService userScheduleService, IUserService userService, SignInManager<IdentityUser> signInManager, IApplicantService applicantService, IApplicationService applicationService)
        {
            _userScheduleService = userScheduleService;
            _userService = userService;
            _applicantService = applicantService;
            _signInManager = signInManager;
            _applicationService = applicationService;
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SchedulerDataViewModel formData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.Warn("Invalid ModelState.");
                    return BadRequest(ModelState);
                }

                // Create a new UserSchedule record for each ApplicantTime in the formData
                foreach (var applicantTime in formData.ApplicantData)
                {
                    // Get the ApplicationId based on the ApplicantId for each ApplicantTime
                    Guid applicationId = _applicationService.GetApplicationIdByApplicantId(applicantTime.ApplicantId);

                    var userSchedule = new UserSchedule
                    {
                        UserId = 1, // Assuming UserId will always be 1
                        JobOpeningId = formData.JobOpeningId,
                        Type = formData.Type,
                        Schedule = DateTime.Parse(formData.Date.ToString() + " " + applicantTime.Time),
                        ApplicationId = applicationId
                    };

                    _userScheduleService.CreateSchedule(userSchedule);
                }

                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View();
            }
        }
    }
}

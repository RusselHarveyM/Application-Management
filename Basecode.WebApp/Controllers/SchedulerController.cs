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
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly SignInManager<IdentityUser> _signInManager;

        public SchedulerController(IUserScheduleService userScheduleService, IUserService userService, SignInManager<IdentityUser> signInManager, IApplicantService applicantService)
        {
            _userScheduleService = userScheduleService;
            _userService = userService;
            _applicantService = applicantService;
            _signInManager = signInManager;
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

                _userScheduleService.CreateSchedules(formData);

                return RedirectToAction("Index", "Dashboard");
            }
            catch
            {
                return View();
            }
        }
    }
}

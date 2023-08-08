using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NToastNotify;

namespace Basecode.WebApp.Controllers;


public class JobController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IToastNotification _toastNotification;
    private readonly UserManager<IdentityUser> _userManager;

    public JobController(IJobOpeningService jobOpeningService, UserManager<IdentityUser> userManager,
        IToastNotification toastNotification)
    {
        _jobOpeningService = jobOpeningService;
        _userManager = userManager;
        _toastNotification = toastNotification;
    }

    /// <summary>
    ///     Retrieves a list of job openings and returns a view with the list.
    /// </summary>
    /// <returns>
    ///     A view with a list of job openings.
    /// </returns>
    public IActionResult Index()
    {
        try
        {
            // Get all jobs currently available.
            var jobOpenings = _jobOpeningService.GetJobs();

            if (jobOpenings.IsNullOrEmpty())
            {
                _logger.Error("No current jobs.");
                return View(new List<JobOpeningViewModel>());
            }

            _logger.Trace("Get Jobs Successfully");
            return View(jobOpenings);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }


    /// <summary>
    ///     Returns a view for creating a new job opening.
    /// </summary>
    /// <returns>
    ///     A view for creating a new job opening.
    /// </returns>
    [Authorize]
    public IActionResult CreateView()
    {
        try
        {
            var model = new JobOpeningViewModel();
            _logger.Trace("Successfuly renders form.");
            return View(model);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    ///     Retrieves a job opening with the given id and returns a view with its details.
    /// </summary>
    /// <param name="id">The id of the job opening to retrieve.</param>
    /// <returns>
    ///     A view with the job opening details or NotFound result if no job opening is found.
    /// </returns>
    public IActionResult JobView(int id)
    {
        try
        {
            var jobOpening = _jobOpeningService.GetById(id);
            if (jobOpening == null)
            {
                var message = "Job Opening [" + id + "] not found!";
                _logger.Error(message);
                _toastNotification.AddInfoToastMessage(message);
                return RedirectToAction("Index");
            }

            _logger.Trace("Job Opening [" + id + "] found.");
            return View(jobOpening);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    ///  Creates the specified job opening.
    /// </summary>
    /// <param name="jobOpening">The job opening.</param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(JobOpeningViewModel jobOpening)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            (ErrorHandling.LogContent logContent, int jobOpeningId) data =
                _jobOpeningService.Create(jobOpening, user.Email);
            //Checks for any validation warning
            if (!data.logContent.Result && data.jobOpeningId > 0)
            {
                _logger.Trace("Create JobOpening succesfully.");

                // Assign logged-in user to the new job opening
                var assignedUser = new List<string> { user.Id };
                _jobOpeningService.UpdateJobOpeningUsers(data.jobOpeningId, assignedUser);
                _toastNotification.AddSuccessToastMessage("Job Created Succesfully.");
                return RedirectToAction("Index");
            }

            //Fails the validation
            _logger.Warn(ErrorHandling.SetLog(data.logContent));
            _toastNotification.AddWarningToastMessage(ErrorHandling.SetLog(data.logContent));
            return View("CreateView", jobOpening);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    ///     Retrieves a job opening with the given id and returns a view for updating it.
    /// </summary>
    /// <param name="id">The id of the job opening to retrieve.</param>
    /// <returns>
    ///     A view for updating the job opening or NotFound result if no job opening is found.
    /// </returns>
    [Authorize]
    public IActionResult UpdateView(int id)
    {
        try
        {
            //Checks if Job Opening Exists.
            var jobOpening = _jobOpeningService.GetById(id);

            if (jobOpening == null)
            {
                var message = "Job Opening [" + id + "] not found!";
                _logger.Trace(message);
                _toastNotification.AddInfoToastMessage(message);
                return RedirectToAction("Index");
            }

            _logger.Trace("Successfully get JobOpening by the Id: { " + id + " }");
            return View(jobOpening);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    ///     Updates an existing job opening and redirects to the Index action if the model state is valid.
    /// </summary>
    /// <param name="jobOpening">The JobOpeningViewModel object to update.</param>
    /// <returns>
    ///     Redirects to the Index action if the model state is valid or returns the same view with the model if not valid.
    /// </returns>
    [HttpPost]
    [Authorize]
    public IActionResult Update(JobOpeningViewModel jobOpening)
    {
        try
        {
            var data = _jobOpeningService.Update(jobOpening, User.Identity?.Name ?? "person1");
            if (!data.Result)
            {
                // Update the job opening
                _logger.Trace("Updated [" + jobOpening.Id + "] successfully.");
                _toastNotification.AddInfoToastMessage("Job Updated.");
                return RedirectToAction("Index");
            }

            _logger.Trace(ErrorHandling.SetLog(data));
            return View("UpdateView", jobOpening);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }


    /// <summary>
    ///     Deletes a job opening with the given id and redirects to the Index action.
    /// </summary>
    /// <param name="id">The id of the job opening to delete.</param>
    /// <returns>
    ///     Redirects to the Index action or returns NotFound result if no job opening is found.
    /// </returns>
    [HttpPost]
    [Authorize]
    public IActionResult Delete(int id)
    {
        try
        {
            //Checks if Job Opening Exists.
            var jobOpening = _jobOpeningService.GetById(id);

            if (jobOpening == null) return NotFound();
            _logger.Trace("Deleted [" + id + "] successfully.");
            _jobOpeningService.Delete(jobOpening);
            _toastNotification.AddSuccessToastMessage("Job Deleted Succesfully.");
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            _toastNotification.AddErrorToastMessage(e.Message);
            return RedirectToAction("Index");
        }
    }
}
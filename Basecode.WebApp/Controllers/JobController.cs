using Basecode.Services.Interfaces;
using Basecode.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using Basecode.Data.ViewModels;
using NLog;
using Basecode.Services.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace Basecode.WebApp.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobOpeningService _jobOpeningService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="JobController" /> class.
        /// </summary>
        /// <param name="jobOpeningService">The job opening service.</param>
        public JobController(IJobOpeningService jobOpeningService)
        {
            _jobOpeningService = jobOpeningService;
        }

        /// <summary>
        /// Retrieves a list of job openings and returns a view with the list.
        /// </summary>
        /// <returns>
        /// A view with a list of job openings.
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
                return StatusCode(500, "Something went wrong.");
            }
        }


        /// <summary>
        /// Returns a view for creating a new job opening.
        /// </summary>
        /// <returns>
        /// A view for creating a new job opening.
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
                return StatusCode(500, "Something went wrong.");
            }

        }

        /// <summary>
        /// Retrieves a job opening with the given id and returns a view with its details.
        /// </summary>
        /// <param name="id">The id of the job opening to retrieve.</param>
        /// <returns>
        /// A view with the job opening details or NotFound result if no job opening is found.
        /// </returns>
        public IActionResult JobView(int id)
        {
            try
            {
                var jobOpening = _jobOpeningService.GetById(id);
                if (jobOpening == null)
                {
                    _logger.Error("Job Opening [" + id + "] not found!");
                    return NotFound();
                }
                _logger.Trace("Job Opening [" + id + "] found.");
                return View(jobOpening);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Creates the specified job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IActionResult Create(JobOpeningViewModel jobOpening)
        {
            try
            {
                string createdBy = "dummy_person";
                var data = _jobOpeningService.Create(jobOpening, createdBy);
                //Checks for any validation warning
                if (!data.Result)
                {
                    _logger.Trace("Create JobOpening succesfully.");
                    return RedirectToAction("Index");
                }
                //Fails the validation
                _logger.Trace(ErrorHandling.SetLog(data));
                return View("CreateView", jobOpening);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }

        }

        /// <summary>
        /// Retrieves a job opening with the given id and returns a view for updating it.
        /// </summary>
        /// <param name="id">The id of the job opening to retrieve.</param>
        /// <returns>
        /// A view for updating the job opening or NotFound result if no job opening is found.
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
                    _logger.Trace("Job Opening [" + id + "] not found!");
                    return NotFound();
                }
                _logger.Trace("Successfully get JobOpening by the Id: { " + id + " }");
                return View(jobOpening);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Updates an existing job opening and redirects to the Index action if the model state is valid.
        /// </summary>
        /// <param name="jobOpening">The JobOpeningViewModel object to update.</param>
        /// <returns>
        /// Redirects to the Index action if the model state is valid or returns the same view with the model if not valid.
        /// </returns>
        [HttpPost]
        [Authorize]
        public IActionResult Update(JobOpeningViewModel jobOpening)
        {
            try
            {

                string updatedBy = "dummy1";
                var data = _jobOpeningService.Update(jobOpening, updatedBy);
                if (!data.Result)
                {
                    // Update the job opening
                    _logger.Trace("Updated [" + jobOpening.Id + "] successfully.");
                    return RedirectToAction("Index");
                }
                _logger.Trace(ErrorHandling.SetLog(data));
                return View("UpdateView", jobOpening);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }


        /// <summary>
        /// Deletes a job opening with the given id and redirects to the Index action.
        /// </summary>
        /// <param name="id">The id of the job opening to delete.</param>
        /// <returns>
        /// Redirects to the Index action or returns NotFound result if no job opening is found.
        /// </returns>
        [HttpPost]
        [Authorize]

        public IActionResult Delete(int id)
        {
            try
            {
                //Checks if Job Opening Exists.
                var jobOpening = _jobOpeningService.GetById(id);

                if (jobOpening == null)
                {
                    return NotFound();
                }
                _logger.Trace("Deleted [" + id + "] successfully.");
                _jobOpeningService.Delete(jobOpening);
                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

    }
}

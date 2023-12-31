﻿using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.WebApp.Controllers;

public class PublicApplicationController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicantService _applicantService;
    private readonly IJobOpeningService _jobOpeningService;

    /// <summary>
    ///     Initializes a new instance of the PublicApplicationController class.
    /// </summary>
    /// <param name="applicantService">An instance of the applicant service.</param>
    /// <param name="jobOpeningService">An instance of the job opening service.</param>
    public PublicApplicationController(IApplicantService applicantService, IJobOpeningService jobOpeningService)
    {
        _applicantService = applicantService;
        _jobOpeningService = jobOpeningService;
    }

    /// <summary>
    ///     Displays the application form for a specific job opening.
    /// </summary>
    /// <param name="jobOpeningId">The ID of the job opening.</param>
    /// <returns>Returns a view with the application form.</returns>
    [HttpGet("/PublicApplication/Index/{jobOpeningId}")]
    public IActionResult Index(int jobOpeningId)
    {
        try
        {
            TempData["jobOpeningId"] = jobOpeningId;
            var model = new ApplicantViewModel();
            _logger.Trace("Successfuly renders form.");
            return View(model);
        }
        catch (Exception e)
        {
            _logger.Error(DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Displays the reference page for an applicant.
    /// </summary>
    /// <param name="applicant">The applicant's information.</param>
    /// <param name="fileUpload">The CV file.</param>
    /// <returns>Returns a view displaying the applicant's information and the uploaded file.</returns>
    public IActionResult Reference(ApplicantViewModel applicant, IFormFile fileUpload)
    {
        try
        {
            if (fileUpload != null)
            {
                var fileExtension = Path.GetExtension(fileUpload.FileName);
                if (fileExtension != ".pdf")
                {
                    TempData["ErrorMessage"] = "Only PDF files are allowed.";
                    return RedirectToAction("Index", new { jobOpeningId = applicant.JobOpeningId });
                }

                using (var memoryStream = new MemoryStream())
                {
                    fileUpload.CopyTo(memoryStream);
                    var fileData = memoryStream.ToArray();
                    var base64FileData = Convert.ToBase64String(fileData);
                    TempData["FileData"] = base64FileData;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select a file.";
                return RedirectToAction("Index", new { jobOpeningId = applicant.JobOpeningId });
            }

            TempData["FileName"] = Path.GetFileName(fileUpload.FileName);
            _logger.Trace("Successfully renders form.");
            return View(applicant);
        }
        catch (Exception e)
        {
            _logger.Error(DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Displays the confirmation page for a submitted application.
    /// </summary>
    /// <param name="applicant">The applicant's information.</param>
    /// <param name="fileName">The name of the uploaded file.</param>
    /// <param name="fileData">The CV of the applicant.</param>
    /// <returns>Returns a view displaying the applicant's information and the uploaded file name.</returns>
    public IActionResult Confirmation(ApplicantViewModel applicant, string fileName, string fileData)
    {
        try
        {
            TempData["FileName"] = fileName;
            TempData["FileData"] = fileData;
            _logger.Trace("Successfuly renders form.");
            return View(applicant);
        }
        catch (Exception e)
        {
            _logger.Error(DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Creates a new applicant and updates the application status.
    /// </summary>
    /// <param name="applicant">The applicant's information.</param>
    /// <param name="fileName">The name of the uploaded file.</param>
    /// <param name="applicantId">The ID of the applicant.</param>
    /// <param name="newStatus">The new application status.</param>
    /// <param name="fileData">The CV of the applicant.</param>
    /// <returns>
    ///     Returns a redirect to the job index page if the applicant is created successfully, otherwise returns the index
    ///     view.
    /// </returns>
    [HttpPost]
    public IActionResult Create(ApplicantViewModel applicant, string fileName, int applicantId, string newStatus,
        string fileData)
    {
        try
        {
            var isJobOpening = _jobOpeningService.GetById(applicant.JobOpeningId);
            if (isJobOpening != null)
            {
                var cv = Convert.FromBase64String(fileData).ToArray();
                applicant.CV = cv;
                (var logContent, var createdApplicantId) = _applicantService.Create(applicant);
                if (!logContent.Result)
                {
                    _logger.Trace("Create Applicant successfully.");

                    return RedirectToAction("Index", "Job");
                }

                _logger.Trace(SetLog(logContent));
            }

            return View("Index");
        }
        catch (Exception e)
        {
            _logger.Error(DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}
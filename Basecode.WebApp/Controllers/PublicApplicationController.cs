using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.WebApp.Controllers
{
    public class PublicApplicationController : Controller
    {
        private readonly IApplicantService _applicantService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly ICharacterReferenceService _characterReferenceService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IApplicationService _applicationService;

        /// <summary>
        /// Initializes a new instance of the PublicApplicationController class.
        /// </summary>
        /// <param name="applicantService">An instance of the applicant service.</param>
        /// <param name="jobOpeningService">An instance of the job opening service.</param>
        /// <param name="characterReferenceService">An instance of the character reference service.</param>
        /// <param name="applicationService">An instance of the application serice </param>
        public PublicApplicationController(IApplicantService applicantService, IJobOpeningService jobOpeningService, ICharacterReferenceService characterReferenceService, IApplicationService applicationService)
        {
            _applicantService = applicantService;
            _characterReferenceService = characterReferenceService;
            _jobOpeningService = jobOpeningService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// Displays the application form for a specific job opening.
        /// </summary>
        /// <param name="jobOpeningId">The ID of the job opening.</param>
        /// <returns>Returns a view with the application form.</returns>
        [HttpGet("/PublicApplication/Index/{jobOpeningId}")]
        public IActionResult Index(int jobOpeningId)
        {
            try
            {
                _logger.Trace("jobId: " + jobOpeningId);
                TempData["jobOpeningId"] = jobOpeningId;
                var model = new ApplicantViewModel();
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
        /// Submits a reference form for a job application.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="middlename"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="birthdate"></param>
        /// <param name="gender"></param>
        /// <param name="nationality"></param>
        /// <param name="street"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="zip"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="jobId"></param>
        /// <param name="fileUpload"></param>
        /// <returns>Returns a view with the reference form.</returns>
        public IActionResult Reference(string firstname,
                                       string middlename,
                                       string lastname,
                                       string age,
                                       string birthdate,
                                       string gender,
                                       string nationality,
                                       string street,
                                       string city,
                                       string province,
                                       string zip,
                                       string phone,
                                       string email,
                                       int jobId,
                                       IFormFile fileUpload)
        {
            try
            {
                _logger.Trace("jobId: " + jobId);
                if (fileUpload != null)
                {
                    string fileExtension = Path.GetExtension(fileUpload.FileName);
                    if (fileExtension != ".pdf")
                    {
                        TempData["ErrorMessage"] = "Only PDF files are allowed.";
                        return RedirectToAction("Index", new { jobOpeningId = jobId });
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        fileUpload.CopyTo(memoryStream);
                        byte[] fileData = memoryStream.ToArray();
                        TempData["FileData"] = fileData;
                        
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Please select a file.";
                    return RedirectToAction("Index", new { jobOpeningId = jobId });
                }
                TempData["jobOpeningId"] = jobId;
                TempData["FileName"] = Path.GetFileName(fileUpload.FileName);
                var model = new ApplicantViewModel
                {
                    Firstname = firstname,
                    Middlename = middlename,
                    Lastname = lastname,
                    Age = Convert.ToInt32(age),
                    Birthdate = DateTime.Parse(birthdate),
                    Gender = gender,
                    Nationality = nationality,
                    Street = street,
                    City = city,
                    Province = province,
                    Zip = zip,
                    Phone = phone,
                    Email = email,
                    JobOpeningId = jobId
                };

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
        /// Displays the confirmation page for the submitted application.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="middlename"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="birthdate"></param>
        /// <param name="gender"></param>
        /// <param name="nationality"></param>
        /// <param name="street"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="zip"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="fileName"></param>
        /// <param name="fileData"></param>
        /// <param name="jobId"></param>
        /// <param name="characterReferences"></param>
        /// <returns>Returns a view with the confirmation page.</returns>
        public IActionResult Confirmation(string firstname,
                                          string middlename,
                                          string lastname,
                                          string age,
                                          string birthdate,
                                          string gender,
                                          string nationality,
                                          string street,
                                          string city,
                                          string province,
                                          string zip,
                                          string phone,
                                          string email,
                                          string fileName,
                                          byte[] fileData,
                                          int jobId,
                                          List<CharacterReferenceViewModel> characterReferences)
        {
            try
            {
                _logger.Trace("jobId: " + jobId);
                TempData["jobOpeningId"] = jobId;
                TempData["FileName"] = fileName;
                TempData["FileData"] = fileData;
                var model = new ApplicantViewModel
                {
                    Firstname = firstname,
                    Middlename = middlename,
                    Lastname = lastname,
                    Age = Convert.ToInt32(age),
                    Birthdate = DateTime.Parse(birthdate),
                    Gender = gender,
                    Nationality = nationality,
                    Street = street,
                    City = city,
                    Province = province,
                    Zip = zip,
                    Phone = phone,
                    Email = email,
                    CV = fileData,
                    CharacterReferences = characterReferences,
                    JobOpeningId = jobId
                };
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
        /// Creates a new applicant for a job opening.
        /// </summary>
        /// <param name="firstname"></param>
        /// <param name="middlename"></param>
        /// <param name="lastname"></param>
        /// <param name="age"></param>
        /// <param name="birthdate"></param>
        /// <param name="gender"></param>
        /// <param name="nationality"></param>
        /// <param name="street"></param>
        /// <param name="city"></param>
        /// <param name="province"></param>
        /// <param name="zip"></param>
        /// <param name="phone"></param>
        /// <param name="email"></param>
        /// <param name="fileName"></param>
        /// <param name="fileData"></param>
        /// <param name="jobId"></param>
        /// <param name="characterReferences"></param>
        /// <param name="applicantId"></param>
        /// <param name="newStatus"></param>
        /// <returns>Returns a view.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(string firstname,
                                    string middlename,
                                    string lastname,
                                    string age,
                                    string birthdate,
                                    string gender,
                                    string nationality,
                                    string street,
                                    string city,
                                    string province,
                                    string zip,
                                    string phone,
                                    string email,
                                    string fileName,
                                    byte[] fileData,
                                    int jobId,
                                    List<CharacterReferenceViewModel> characterReferences, int applicantId, string newStatus)
        {
            newStatus = "Success";
            try
            {
                _logger.Trace("jobId: " + jobId);
                var applicant = new ApplicantViewModel
                {
                    Firstname = firstname,
                    Middlename = middlename,
                    Lastname = lastname,
                    Age = Convert.ToInt32(age),
                    Birthdate = DateTime.Parse(birthdate),
                    Gender = gender,
                    Nationality = nationality,
                    Street = street,
                    City = city,
                    Province = province,
                    Zip = zip,
                    Phone = phone,
                    Email = email,
                    CV = fileData,
                    CharacterReferences = characterReferences,
                    JobOpeningId = jobId
                };
                var isJobOpening = _jobOpeningService.GetById(applicant.JobOpeningId);
                if (isJobOpening != null)
                {
                    (LogContent logContent, int createdApplicantId) = _applicantService.Create(applicant);
                    if (!logContent.Result)
                    {
                        _logger.Trace("Create Applicant successfully.");

                        // Send email notifications
                        await _applicationService.UpdateApplicationStatus(createdApplicantId, newStatus);

                        return RedirectToAction("Index", "Job");
                    }
                    _logger.Trace(ErrorHandling.SetLog(logContent));
                }
                return View("Index");
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}

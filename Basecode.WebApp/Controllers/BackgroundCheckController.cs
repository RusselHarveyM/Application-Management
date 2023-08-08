using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers;

public class BackgroundCheckController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicantService _applicantService;
    private readonly IApplicationService _applicationService;
    private readonly IBackgroundCheckService _backgroundCheckService;
    private readonly ICharacterReferenceService _characterReferenceService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IUserService _userService;
    private readonly TokenHelper _tokenHelper;

    public BackgroundCheckController(ICharacterReferenceService characterReferenceService,
        IApplicantService applicantService,
        IApplicationService applicationService, IJobOpeningService jobOpeningService,
        IBackgroundCheckService backgroundCheckService,
        IEmailSendingService emailSendingService, IUserService userService, IConfiguration config)
    {
        _characterReferenceService = characterReferenceService;
        _applicantService = applicantService;
        _applicationService = applicationService;
        _jobOpeningService = jobOpeningService;
        _backgroundCheckService = backgroundCheckService;
        _emailSendingService = emailSendingService;
        _userService = userService;
        _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
    }

    [Route("/BackgroundCheck/Form/{token}")]
    public IActionResult Index(string token)
    {
        _logger.Trace("Enter form successfully");
        try
        {
            //Check token is for request reference related
            var tokenClaims = _tokenHelper.GetTokenClaims(token, "RequestReference");
            if (tokenClaims.Count == 0)
            {
                _logger.Warn("Invalid or Expired token");
                return RedirectToAction("Index", "Home");
            }
            //Check if value is given
            if (tokenClaims.TryGetValue("userId", out var userIdRetrieve) && tokenClaims.TryGetValue("characterReferenceId", out var characterReferenceIdRetrieve))
            {
                if (int.TryParse(userIdRetrieve, out int userId) && int.TryParse(characterReferenceIdRetrieve, out int characterReferenceId))
                {
                    //Check first if referee already answered form
                    var isAnswered = _backgroundCheckService.GetBackgroundByCharacterRefId(characterReferenceId);
                    if (isAnswered != null)
                    {
                        ViewBag.IsFormSubmitted = false;
                        return View("Redirection");
                    }

                    //Get applicant id
                    var applicantId = _characterReferenceService.GetApplicantIdByCharacterReferenceId(characterReferenceId);
                    if (applicantId <= 0) return NotFound();
                    //Get applicant
                    var applicantDetails = _applicantService.GetApplicantById(applicantId);
                    //Get application
                    var applicationDetails = _applicationService.GetApplicationByApplicantId(applicantId);
                    //Get job
                    var jobDetails = _jobOpeningService.GetById(applicationDetails!.JobOpeningId);
                    //Ready the applicant details for view
                    ViewData["bgApplicantLastname"] = applicantDetails.Lastname;
                    ViewData["bgApplicantFirstname"] = applicantDetails.Firstname;
                    ViewData["bgApplicantJob"] = jobDetails.Title;
                    ViewData["bgApplicantDate"] = applicationDetails.ApplicationDate.ToShortDateString();
                    ViewData["bgcrId"] = characterReferenceId;
                    ViewData["bgUserId"] = userId;
                }
                else
                {
                    _logger.Warn("Invalid Id claims");
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }

        return View();
    }

    [Route("/BackgroundCheck/FormOk")]
    public async Task<ActionResult> FormOk(BackgroundCheckFormViewModel data)
    {
        try
        {
            _logger.Trace("Form Submitted Successfully");
            _backgroundCheckService.Create(data);
            ViewBag.IsFormSubmitted = true;

            return View("Redirection");
        }
        catch(Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}
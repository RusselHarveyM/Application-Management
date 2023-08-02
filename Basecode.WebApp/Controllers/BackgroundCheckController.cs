using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
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
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public BackgroundCheckController(ICharacterReferenceService characterReferenceService,
        IApplicantService applicantService,
        IApplicationService applicationService, IJobOpeningService jobOpeningService,
        IBackgroundCheckService backgroundCheckService,
        SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _characterReferenceService = characterReferenceService;
        _applicantService = applicantService;
        _applicationService = applicationService;
        _jobOpeningService = jobOpeningService;
        _backgroundCheckService = backgroundCheckService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    //[Route("/BackgroundCheck/Form/{characterReferenceId}/{userId}")]
    public IActionResult Index()
    {
        //to be implemented
        return View();
    }

    [Route("/BackgroundCheck/Form/{characterReferenceId}/{userId}")]
    public IActionResult Form(int characterReferenceId, int userId)
    {
        _logger.Trace("BackgroundCheck Controller | Enter form successfully");

        try
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
            //Get character reference
            var getReference = _characterReferenceService.GetCharacterReferenceById(characterReferenceId);
            //Ready the applicant details for view
            ViewData["bgApplicantLastname"] = applicantDetails.Lastname;
            ViewData["bgApplicantFirstname"] = applicantDetails.Firstname;
            ViewData["bgApplicantJob"] = jobDetails.Title;
            ViewData["bgApplicantDate"] = applicationDetails.ApplicationDate.ToShortDateString();
            ViewData["bgcrId"] = characterReferenceId;
            ViewData["bgUserId"] = userId;
            //Ready the referees details for view
            var slicedName = getReference.Name.Split(' ');
            ViewData["bgRefFirstname"] = slicedName[0] ?? " ";
            ViewData["bgRefLastname"] = slicedName[1] ?? " ";
            ViewData["bgRefEmail"] = getReference.Email;
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
        var whoIsSigned = _signInManager.IsSignedIn(User);
        if (whoIsSigned)
        {
            var userSign = await _userManager.GetUserAsync(User);
        }

        _logger.Trace("Form Submitted Successfully");
        _backgroundCheckService.Create(data);
        ViewBag.IsFormSubmitted = true;
        return View("Redirection");
    }
}
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class BackgroundCheck : Controller
    {
        private readonly ICharacterReferenceService _characterReferenceService;
        private readonly IApplicantService _applicantService;
        private readonly IApplicationService _applicationService;
        private readonly IJobOpeningService _jobOpeningService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BackgroundCheck(ICharacterReferenceService characterReferenceService, IApplicantService applicantService,
            IApplicationService applicationService, IJobOpeningService jobOpeningService)
        {
            _characterReferenceService = characterReferenceService;
            _applicantService = applicantService;
            _applicationService = applicationService;
            _jobOpeningService = jobOpeningService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Background/Form/{characterReferenceId}/{userId}")]
        public IActionResult BackgroundForm(int characterReferenceId, int userId)
        {
            //Get applicant id
            var applicantId = _characterReferenceService.GetApplicantIdByCharacterReferenceId(characterReferenceId);
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

            return View();
        }
        
    }
}

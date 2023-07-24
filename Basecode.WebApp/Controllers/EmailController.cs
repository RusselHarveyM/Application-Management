using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Basecode.WebApp.Controllers
{
    public class EmailController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IUserService _userService;
        private readonly IApplicantService _applicantService;
        private readonly IEmailService _emailService;
        private readonly IApplicationService _applicationService;

        public EmailController(ITrackService trackService, IUserService userService, IApplicantService applicantService, IApplicationService application)
        {
            _trackService = trackService;
            _userService = userService;
            _applicantService = applicantService;
            _applicationService = application;
        }

        public IActionResult SendApprovalEmail(Applicant applicant, Guid appId, string email, string status,string newStatus, string mailType)
        {
            var application = _applicationService.GetApplicationById(appId);
            var foundUser = _userService.GetByEmail(email);
            _applicantService.UpdateApplication(application, foundUser, "approved", status);
            _trackService.UpdateTrackStatusEmail(applicant, appId, foundUser.Id, newStatus, mailType);
            return RedirectToAction("ShortListView", "Dashboard");
        }
    }
}

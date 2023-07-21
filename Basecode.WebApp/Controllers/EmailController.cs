using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Basecode.WebApp.Controllers
{
    public class EmailController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public EmailController(ITrackService trackService, IUserService userService)
        {
            _trackService = trackService;
            _userService = userService;
        }

        public IActionResult SendApprovalEmail(Applicant applicant, Guid appId, string email, string newStatus, string mailType)
        {
            var foundUser = _userService.GetByEmail(email);
            _trackService.UpdateTrackStatusEmail(applicant, appId, foundUser.Id, newStatus, mailType);
            return RedirectToAction("ShortListView", "Dashboard");
        }
    }
}

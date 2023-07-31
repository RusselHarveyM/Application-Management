using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class JobOfferController : Controller
    {
        private readonly TokenHelper _tokenHelper;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICurrentHireService _currentHireService;
        private const string SecretKey = "CDC1CAAACAA3269755F5EC44C7202F0055C9C322AEB5C4B6103F6E9C11EF136F";

        public JobOfferController(ICurrentHireService currentHireService)
        {
            _currentHireService = currentHireService;
            _tokenHelper = new TokenHelper(SecretKey);
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Accepts the job offer.
        /// </summary>
        [Route("CurrentHire/AcceptOffer/{token}")]
        public IActionResult AcceptOffer(string token)
        {
            try
            {
                ViewBag.IsOfferAccepted = false;
                int userOfferId = _tokenHelper.GetIdFromToken(token, "accept");
                if (userOfferId == 0)
                {
                    _logger.Warn("Invalid or expired token.");
                    return View();
                }

                var data = _currentHireService.AcceptOffer(userOfferId);
                if (!data.Result)
                {
                    _logger.Trace("User Offer [" + userOfferId + "] has been successfully accepted.");
                    ViewBag.IsOfferAccepted = true;
                }

                return View();
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }
    }
}

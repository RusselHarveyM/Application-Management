using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Basecode.WebApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _service;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AdminController(IAdminService service) {
            _service = service;
        }    
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRole()
        {
             return View("RoleManagement/CreateRole");
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel createRoleViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    IdentityResult result = await _service.CreateRole(createRoleViewModel.RoleName);

                    if(result.Succeeded || result! == null)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }

                return View();
            }
            catch (Exception e)
            {
                _logger.Error("Something went wrong " + e.Message);
                return StatusCode(500);
            }
        }
    }
}

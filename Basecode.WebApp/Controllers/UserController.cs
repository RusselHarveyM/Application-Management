﻿using Microsoft.AspNetCore.Mvc;

namespace Basecode.WebApp.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

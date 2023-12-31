﻿using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;

namespace Basecode.WebApp.Controllers;

[Authorize]
public class UserController : Controller
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IEmailSchedulerService _emailSchedulerService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IUserService _service;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserController" /> class.
    /// </summary>
    /// <param name="service">The User service.</param>
    public UserController(IUserService service, IJobOpeningService jobOpeningService,
        IEmailSchedulerService emailSchedulerService)
    {
        _service = service;
        _jobOpeningService = jobOpeningService;
        _emailSchedulerService = emailSchedulerService;
    }

    /// <summary>
    ///     Displays the list of all users.
    /// </summary>
    /// <returns>A view containing all users as a list of UserViewModel objects.</returns>
    public IActionResult Index()
    {
        try
        {
            var data = _service.RetrieveAll();

            if (data.IsNullOrEmpty())
            {
                _logger.Info("No users found.");
                return View(new List<UserViewModel>());
            }

            _logger.Trace("Successfully retrieved all users.");
            return View(data);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Displays the modal for adding a new user.
    /// </summary>
    /// <returns>A partial view with a User model.</returns>
    [HttpGet]
    public ActionResult AddView()
    {
        try
        {
            var userModel = new UserViewModel();
            return PartialView("~/Views/User/_AddView.cshtml", userModel);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Adds a new user to the system.
    /// </summary>
    /// <param name="user">User object representing the user to be added.</param>
    /// <returns>Redirect to the Index() action to display the list of users.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel user)
    {
        try
        {
            // Validate through data annotations
            if (!ModelState.IsValid)
            {
                _logger.Warn("Submitted User failed at least one data annotation validation.");
                return BadRequest(ModelState);
            }

            // Create the new user
            var data = await _service.Create(user);

            if (!data.Result)
            {
                _logger.Trace("Successfully created a new user.");
                return Ok();
            }

            _logger.Warn(ErrorHandling.SetLog(data));
            ModelState.AddModelError("Email", "The Email Address format is invalid.");

            // Call the service method to get the validation errors
            var validationErrors = _service.GetValidationErrors(ModelState);

            return BadRequest(Json(validationErrors));
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Displays the modal for updating an existing user.
    /// </summary>
    /// <param name="id">Integer representing the ID of the user to be updated.</param>
    /// <returns>A partial view and User object.</returns>
    [HttpGet]
    public async Task<ActionResult> UpdateView(int id)
    {
        try
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                _logger.Error("User [" + id + "] not found.");
                return NotFound();
            }

            var vmData = new UserUpdateViewModel
            {
                Id = data.Id,
                AspId = data.AspId,
                Username = data.Username,
                Fullname = data.Fullname,
                Email = data.Email,
                Password = data.Password,
                Role = data.Role
            };
            _logger.Trace("Successfully retrieved user by ID: [" + id + "].");
            return PartialView("~/Views/User/_UpdateView.cshtml", vmData);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Updates an existing user in the system.
    /// </summary>
    /// <param name="user">UserUpdateViewModel object representing the user with updated information.</param>
    /// <returns>Redirect to the Index() action to display the list of users.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UserUpdateViewModel user)
    {
        try
        {
            // Validate through data annotations
            if (!ModelState.IsValid)
            {
                _logger.Warn("Submitted User failed at least one data annotation validation.");
                return BadRequest(ModelState);
            }

            // Update the user
            var data = await _service.Update(user);

            if (!data.Result)
            {
                _logger.Trace("Successfully updated user [" + user.Id + "].");
                return Ok();
            }

            _logger.Warn(ErrorHandling.SetLog(data));
            ModelState.AddModelError("Email", "The Email Address format is invalid.");

            // Call the service method to get the validation errors
            var validationErrors = _service.GetValidationErrors(ModelState);

            return BadRequest(Json(validationErrors));
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Displays the modal for deleting an existing user.
    /// </summary>
    /// <param name="id">Integer representing the ID of the user to be deleted.</param>
    /// <returns>A partial view and User object.</returns>
    [HttpGet]
    public async Task<ActionResult> DeleteView(int id)
    {
        try
        {
            var data = await _service.GetByIdAsync(id);

            if (data == null)
            {
                _logger.Error("User [" + id + "] not found.");
                return NotFound();
            }

            _logger.Trace("Successfully retrieved user by ID: [" + id + "].");
            return PartialView("~/Views/User/_DeleteView.cshtml", data);
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }

    /// <summary>
    ///     Deletes a user from the system.
    /// </summary>
    /// <param name="id">Integer representing the ID of the user to be deleted.</param>
    /// <returns>Redirect to the Index() action to display the updated list of users.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _service.GetByIdAsync(id);

            if (user == null)
            {
                _logger.Error("User [" + id + "] not found.");
                return NotFound();
            }

            await _service.Delete(user);
            _logger.Trace("Successfully deleted user [" + id + "].");
            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            _logger.Error(ErrorHandling.DefaultException(e.Message));
            return StatusCode(500, "Something went wrong.");
        }
    }
}
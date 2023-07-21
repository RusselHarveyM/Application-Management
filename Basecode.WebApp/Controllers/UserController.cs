using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System.Text;

namespace Basecode.WebApp.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IAssignInterviewerService _assignInterviewerService;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="service">The User service.</param>
        public UserController(IUserService service, IJobOpeningService jobOpeningService, IAssignInterviewerService assignInterviewer, IEmailService emailService)
        {
            _service = service;
            _jobOpeningService = jobOpeningService;
            _assignInterviewerService = assignInterviewer;
            _emailService = emailService;
        }

        /// <summary>
        /// Displays the list of all users.
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
        /// Displays the modal for adding a new user.
        /// </summary>
        /// <returns>A partial view with a User model.</returns>
        [HttpGet]
        public ActionResult AddView()
        {
            try
            {
                var userModel = new User();
                return PartialView("~/Views/User/_AddView.cshtml", userModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }   
        }

        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="user">User object representing the user to be added.</param>
        /// <returns>Redirect to the Index() action to display the list of users.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
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
                var data = _service.Create(user);
                
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
        /// Displays the modal for updating an existing user.
        /// </summary>
        /// <param name="id">Integer representing the ID of the user to be updated.</param>
        /// <returns>A partial view and User object.</returns>
        [HttpGet]
        public ActionResult UpdateView(int id)
        {
            try
            {
                var data = _service.GetById(id);

                if (data == null)
                {
                    _logger.Error("User [" + id + "] not found.");
                    return NotFound();
                }

                _logger.Trace("Successfully retrieved user by ID: [" + id + "].");
                return PartialView("~/Views/User/_UpdateView.cshtml", data);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Updates an existing user in the system.
        /// </summary>
        /// <param name="user">User object representing the user with updated information.</param>
        /// <returns>Redirect to the Index() action to display the list of users.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(User user)
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
                var data = _service.Update(user);

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

        [HttpGet]
        public ActionResult AssignView(int id)
        {
            try
            {
                var jobOpenings = _jobOpeningService.GetJobs();
                var data = _service.GetById(id);
                //var jobOpenings = _jobOpeningService.GetJobs();

                if (data == null)
                {
                    _logger.Error("User [" + id + "] not found.");
                    return NotFound();
                }

                var viewModel = new AssignViewModel
                {
                    UserId = id,
                    JobOpenings = jobOpenings
                };
                _logger.Trace("Successfully retrieved user by ID: [" + id + "].");
                return PartialView("~/Views/User/_AssignView.cshtml", viewModel);
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(int UserId, int jobOpeningId)
        {
            try
            {
                //Get User Id
                var data = _service.GetById(UserId);
                var email = data.Email;

                //Get Job Opening
                var jobOpenings = _jobOpeningService.GetById(jobOpeningId);
                var jobposition = jobOpenings.Title;

                var checkJobs = _assignInterviewerService.GetJobs();
                foreach( var job in checkJobs ) 
                { 
                    if( job.JobPosition == jobposition && job.Email == email)
                    {
                        _logger.Error("Interviewer already assigned to [" + jobposition + "]");
                        return RedirectToAction("Index");
                    }
                }
                // Perform the assignment
                _assignInterviewerService.Create(jobposition, email);

                // Use switch case for different roles
                switch (data.Role)
                {
                    case "Deployment Team":
                        _emailService.ScheduleInterview(email, data.Fullname, data.Username, data.Password, jobposition);
                        break;

                    case "Human Resources":
                        _emailService.ScheduleForHR(email, data.Fullname, data.Username, data.Password, jobposition);
                        break;

                    case "Technical":
                        _emailService.ScheduleForTechnical(email, data.Fullname, data.Username, data.Password, jobposition);
                        break;

                    default:
                        // Handle the default case if the role doesn't match any case
                        break;
                }

                // Return success status
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.Error(ErrorHandling.DefaultException(e.Message));
                return StatusCode(500, "Something went wrong.");
            }
        }

        /// <summary>
        /// Displays the modal for deleting an existing user.
        /// </summary>
        /// <param name="id">Integer representing the ID of the user to be deleted.</param>
        /// <returns>A partial view and User object.</returns>
        [HttpGet]
        public ActionResult DeleteView(int id)
        {
            try
            {
                var data = _service.GetById(id);

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
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="id">Integer representing the ID of the user to be deleted.</param>
        /// <returns>Redirect to the Index() action to display the updated list of users.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _service.GetById(id);

                if (user == null)
                {
                    _logger.Error("User [" + id + "] not found.");
                    return NotFound();
                }

                _service.Delete(user);
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
}

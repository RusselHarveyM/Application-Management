using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class UserService : ErrorHandling, IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IJobOpeningService _jobOpeningService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public UserService(IUserRepository repository, IJobOpeningService jobOpeningService)
        {
            _repository = repository;
            _jobOpeningService = jobOpeningService;
        }

        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>
        /// A list of UserViewModel objects representing all users available.
        /// </returns>
        public List<UserViewModel> RetrieveAll()
        {
            var data = _repository.RetrieveAll().Select(s => new UserViewModel
            {
                Id = s.Id,
                Fullname = s.Fullname,
                Username = s.Username,
                Email = s.Email,
                Role = s.Role,
            }).ToList();

            return data;
        }

        /// <summary>
        /// Creates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <response code="400">User details are invalid</response>
        public LogContent Create(User user)
        {
            LogContent logContent = new LogContent();
            logContent = CheckUser(user);

            if (logContent.Result == false)
            {
                _repository.Create(user);
            }

            return logContent;
        }

        /// <summary>
        /// Retrieves a specific user based on the provided ID.
        /// </summary>
        /// <param name="id">Represents the ID of the user to fetch.</param>
        /// <returns>
        /// A User object corresponding to the matching ID.
        /// </returns>
        public User GetById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">Represents the user with updated information.</param>
        public LogContent Update(User user)
        {
            LogContent logContent = new LogContent();
            logContent = CheckUser(user);

            if (logContent.Result == false)
            {
                var userToBeUpdated = _repository.GetById(user.Id);
                userToBeUpdated.Fullname = user.Fullname;
                userToBeUpdated.Username = user.Username;
                userToBeUpdated.Email = user.Email;
                userToBeUpdated.Password = user.Password;
                userToBeUpdated.Role = user.Role;
                _repository.Update(userToBeUpdated);
            }

            return logContent;
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void Delete(User user)
        {
            _repository.Delete(user);
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <returns>Dictionary containing the validation errors.</returns>
        /// <exception cref="Basecode.Data.Constants.Exception">ModelState is empty</exception>
        public Dictionary<string, string> GetValidationErrors(ModelStateDictionary modelState)
        {
            var validationErrors = new Dictionary<string, string>();

            foreach (var key in modelState.Keys)
            {
                var modelStateEntry = modelState[key];

                foreach (var error in modelStateEntry.Errors)
                {
                    validationErrors.Add(key, error.ErrorMessage);
                }
            }

            return validationErrors;
        }

        public List<HRUserViewModel> GetAllUsersWithLinkStatus(int jobOpeningId)
        {
            var allUsers = _repository.RetrieveAll();

            // Get the users linked to the specified JobOpening
            var linkedUserIds = _jobOpeningService.GetLinkedUserIds(jobOpeningId); // Replace _jobOpeningRepository with your actual repository instance to get linked users

            // Create a new HRUserViewModel list with link status
            var usersWithLinkStatus = allUsers.Select(user => new HRUserViewModel
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Email = user.Email,
                IsLinkedToJobOpening = linkedUserIds.Contains(user.Id)
            }).ToList();

            return usersWithLinkStatus;
        }
    }
}
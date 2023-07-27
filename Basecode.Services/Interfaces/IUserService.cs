using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Retrieves a list of all users.
        /// </summary>
        /// <returns>
        /// A list of UserViewModel objects representing all users available.
        /// </returns>
        List<UserViewModel> RetrieveAll();

        /// <summary>
        /// Creates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<LogContent> Create(UserViewModel user);

        /// <summary>
        /// Retrieves a specific user based on the provided ID.
        /// </summary>
        /// <param name="id">Represents the ID of the user to fetch.</param>
        /// <returns>
        /// A User object corresponding to the matching ID.
        /// </returns>
        User GetById(int id);

        /// <summary>
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<User> GetByIdAsync(int id);
        /// <summary>
        /// Gets the by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        User GetByEmail(string email);

        /// <summary>
        /// Updates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        Task<LogContent> Update(UserUpdateViewModel user);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        Task Delete(User user);

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <returns>
        /// Dictionary containing the validation errors.
        /// </returns>
        Dictionary<string, string> GetValidationErrors(ModelStateDictionary modelState);

        /// <summary>
        /// Gets all users and their link status to a job opening.
        /// </summary>
        /// <param name="jobOpeningId">The job opening id.</param>
        /// <returns></returns>
        List<HRUserViewModel> GetAllUsersWithLinkStatus(int jobOpeningId);

        /// <summary>
        /// Gets the linked job openings.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        List<JobOpeningBasicViewModel> GetLinkedJobOpenings(string userAspId);

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        IdentityUser FindUser(string userName);
        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        Task<IdentityResult> CreateRole(string roleName);
        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<IdentityUser> FindUser(string username, string password);
        /// <summary>
        /// Finds the user asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<IdentityUser> FindUserAsync(string userName, string password);

        /// <summary>
        /// Gets the user identifier by ASP identifier.
        /// </summary>
        /// <param name="aspId">The ASP identifier.</param>
        /// <returns></returns>
        int GetUserIdByAspId(string aspId);
    }
}

using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves all users from the User table.
        /// </summary>
        /// <returns>
        /// A queryable collection of objects of type User.
        /// </returns>
        IQueryable<User> RetrieveAll();

        /// <summary>
        /// Adds a new user into the User table.
        /// </summary>
        /// <param name="user">Represents the user to be added.</param>
        Task Create(User user);

        /// <summary>
        /// Retrieves a user from the User table based on the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>
        /// A User object corresponding to the matching ID.
        /// </returns>
        User GetById(int id);


        /// <summary>
        /// Gets the by email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        User GetByEmail(string email);

        /// <summary>
        /// Updates an existing user in the User table.
        /// </summary>
        /// <param name="user">Represents the user with updated information.</param>
        void Update(User user);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        void Delete(User user);

        /// <summary>
        /// Gets the job openings assigned to the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<JobOpeningBasicViewModel> GetLinkedJobOpenings(int userId);

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        IdentityUser FindUser(string userName);

        /// <summary>
        /// Registers the user to ASP.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="email">The email.</param>
        /// <param name="role">The role.</param>
        /// <returns></returns>
        Task RegisterUserToAsp(string username, string password, string email, string role);
        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        Task<IdentityResult> CreateRole(string roleName);
        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<IdentityUser> FindUser(string userName, string password);
        /// <summary>
        /// Finds the user asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<IdentityUser> FindUserAsync(string userName, string password);
    }
}

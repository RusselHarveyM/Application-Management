using Basecode.Data.Models;
using Basecode.Data.ViewModels;
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
        void Create(User user);

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
    }
}

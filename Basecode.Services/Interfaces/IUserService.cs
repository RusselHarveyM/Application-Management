using Basecode.Data.Models;
using Basecode.Data.ViewModels;
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
        /// <returns>A list of UserViewModel objects representing all users available.</returns>
        List<UserViewModel> RetrieveAll();

        /// <summary>
        /// Creates the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        LogContent Create(User user);

        /// <summary>
        /// Retrieves a specific user based on the provided ID.
        /// </summary>
        /// <param name="id">Represents the ID of the user to fetch.</param>
        /// <returns>A User object corresponding to the matching ID.</returns>
        User GetById(int id);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">Represents the user with updated information.</param>
        LogContent Update(User user);

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        void Delete(User user);

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <param name="modelState">State of the model.</param>
        /// <returns>Dictionary containing the validation errors.</returns>
        Dictionary<string, string> GetValidationErrors(ModelStateDictionary modelState);
    }
}

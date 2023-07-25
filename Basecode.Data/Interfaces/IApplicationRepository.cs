using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    /// <summary>
    /// Defines methods for managing application data.
    /// </summary>
    public interface IApplicationRepository
    {
        /// <summary>
        /// Retrieves an application by its ID.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>The application with the specified ID, or null if not found.</returns>
        Application GetById(Guid id);

        /// <summary>
        /// Creates a new application entry.
        /// </summary>
        /// <param name="application">The application to be added.</param>
        void CreateApplication(Application application);

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="application">The application to update.</param>
        void UpdateApplication(Application application);
    }
}

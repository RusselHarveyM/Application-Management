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
        Guid CreateApplication(Application application);

        IQueryable<Application> GetAll();

        /// <summary>
        /// Updates an existing application.
        /// </summary>
        /// <param name="application">The application to update.</param>
        void UpdateApplication(Application application);

        /// <summary>
        /// Gets the applications by ids.
        /// </summary>
        /// <param name="applicationIds">The application ids.</param>
        /// <returns></returns>
        List<Application> GetApplicationsByIds(List<Guid> applicationIds);

        /// <summary>
        /// Gets the application id based on the applicant id.
        /// </summary>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        Guid GetApplicationIdByApplicantId(int applicantId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IQueryable<Application> GetAll();
    }
}

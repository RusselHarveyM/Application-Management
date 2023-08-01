using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IApplicationService
    {
        /// <summary>
        /// Retrieves an application by its ID.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>
        /// The application with the specified ID, or null if not found.
        /// </returns>
        ApplicationViewModel GetById(Guid id);

        /// <summary>
        /// Gets the application by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Application GetApplicationById(Guid id);

        /// <summary>
        /// Gets the application with complete relations by identifier.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        Application? GetApplicationWithAllRelationsById(Guid applicationId);

        /// <summary>
        /// Gets the shorlisted applicatons.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns></returns>
        List<Application> GetShorlistedApplicatons(string stage, int jobId);

        /// <summary>
        /// Creates the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        void Create(Application application);

        /// <summary>
        /// Creates the with identifier.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        Guid CreateWithId(Application application);

        /// <summary>
        /// Updates the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        LogContent Update(Application application);

        /// <summary>
        /// Gets the applications by ids.
        /// </summary>
        /// <param name="applicationIds">The application ids.</param>
        /// <returns></returns>
        List<Application> GetApplicationsByIds(List<Guid> applicationIds);

        /// <summary>
        /// Gets the application id based on the applicant id.
        /// </summary>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <returns></returns>
        Guid GetApplicationIdByApplicantId(int applicantId);

        /// <summary>
        /// Gets the application by applicant identifier.
        /// </summary>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <returns></returns>
        Application? GetApplicationByApplicantId(int applicantId);
    }
}

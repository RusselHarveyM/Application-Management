using Basecode.Data.Models;

namespace Basecode.Data.Interfaces;

public interface IApplicationRepository
{
    /// <summary>
    ///     Retrieves an application by its ID.
    /// </summary>
    /// <param name="id">The ID of the application to retrieve.</param>
    /// <returns>
    ///     The application with the specified ID, or null if not found.
    /// </returns>
    Application GetById(Guid id);

    /// <summary>
    ///     Gets the application with complete relations by identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    Application? GetApplicationWithAllRelationsById(Guid applicationId);

    /// <summary>
    ///     Creates a new application entry.
    /// </summary>
    /// <param name="application">The application to be added.</param>
    /// <returns></returns>
    Guid CreateApplication(Application application);

    /// <summary>
    ///     Gets all.
    /// </summary>
    /// <returns></returns>
    IQueryable<Application> GetAll();

    /// <summary>
    ///     Updates an existing application.
    /// </summary>
    /// <param name="application">The application to update.</param>
    void UpdateApplication(Application application);

    /// <summary>
    ///     Gets the applications by ids.
    /// </summary>
    /// <param name="applicationIds">The application ids.</param>
    /// <returns></returns>
    List<Application> GetApplicationsByIds(List<Guid> applicationIds);

    /// <summary>
    ///     Gets the application id based on the applicant id.
    /// </summary>
    /// <param name="applicantId">The applicant identifier.</param>
    /// <returns></returns>
    Guid GetApplicationIdByApplicantId(int applicantId);
}
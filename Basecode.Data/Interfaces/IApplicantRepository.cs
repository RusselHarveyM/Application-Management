using Basecode.Data.Models;
using Basecode.Data.ViewModels;

namespace Basecode.Data.Interfaces
{
    /// <summary>
    /// Represents an interface for the Applicant repository.
    /// </summary>
    public interface IApplicantRepository
    {
        /// <summary>
        /// Retrieves all applicants.
        /// </summary>
        /// <returns>An IQueryable of Applicant.</returns>
        IQueryable<Applicant> GetAll();

        /// <summary>
        /// Retrieves an applicant by its ID.
        /// </summary>
        /// <param name="id">The ID of the applicant.</param>
        /// <returns>The Applicant object.</returns>
        Applicant GetById(int id);

        Applicant GetByIdAll(int id);

        /// <summary>
        /// Creates a new applicant.
        /// </summary>
        /// <param name="applicant">The Applicant object to create.</param>
        /// <returns>The ID of the created applicant.</returns>
        int CreateApplicant(Applicant applicant);

        /// <summary>
        /// Gets the applicants linked to a job opening.
        /// </summary>
        /// <param name="jobOpeningId"></param>
        /// <returns></returns>
        IQueryable<Applicant> GetApplicantsByJobOpeningId(int jobOpeningId);

        /// <summary>
        /// Gets the applicant by application identifier.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        Applicant GetApplicantByApplicationId(Guid applicationId);

        /// <summary>
        /// This method retrieves a list of applicant names along with their corresponding email addresses and job titles.
        /// It performs a series of database joins to retrieve the required information from the underlying data context.
        /// </summary>
        /// <returns>A list of tuples, each containing the applicant's full name, email address, and job title.</returns>
        List<(string Name, string Email, string Title)> GetApplicantNameAndJobTitle();
    }
}

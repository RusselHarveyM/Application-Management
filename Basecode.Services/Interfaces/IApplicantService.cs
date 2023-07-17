using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    /// <summary>
    /// Represents an interface for the Applicant service.
    /// </summary>
    public interface IApplicantService
    {
        /// <summary>
        /// Retrieves a list of all applicants.
        /// </summary>
        /// <returns>
        /// A list of Applicant objects.
        /// </returns>
        List<Applicant> GetApplicants();

        /// <summary>
        /// Retrieves an applicant by its ID.
        /// </summary>
        /// <param name="id">The ID of the applicant.</param>
        /// <returns>
        /// The Applicant object.
        /// </returns>
        Applicant GetApplicantById(int id);

        /// <summary>
        /// Updates the application.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        void UpdateApplication(Application application, User user, string choice, string newStatus);

        /// <summary>
        /// Creates a new applicant based on the provided applicant data.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <returns>
        /// Returns a tuple with the log content and the ID of the created applicant.
        /// </returns>
        (LogContent, int) Create(ApplicantViewModel applicant);
    }
}

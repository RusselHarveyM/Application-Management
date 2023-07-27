using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
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
        /// Gets the applicant by identifier all.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Applicant GetApplicantByIdAll(int id);

        /// <summary>
        /// Updates the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="choice">The choice.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task UpdateApplication(Application application, User user, string choice, string newStatus);

        /// <summary>
        /// Creates a new applicant based on the provided applicant data.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <returns>
        /// Returns a tuple with the log content and the ID of the created applicant.
        /// </returns>
        Task<(LogContent, int)> Create(ApplicantViewModel applicant);

        /// <summary>
        /// Gets the applicants by the job opening id.
        /// </summary>
        /// <param name="jobOpeningId">The job opening id.</param>
        /// <returns></returns>
        List<ApplicantStatusViewModel> GetApplicantsByJobOpeningId(int jobOpeningId);

        /// <summary>
        /// Gets the applicants with rejected or no schedule.
        /// </summary>
        /// <returns></returns>
        List<ApplicantStatusViewModel> GetApplicantsWithRejectedOrNoSchedule();

        /// <summary>
        /// Gets the applicant by application identifier.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        Applicant GetApplicantByApplicationId(Guid applicationId);
    }
}

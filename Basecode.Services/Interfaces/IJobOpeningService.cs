using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface IJobOpeningService
{
    /// <summary>
    ///     Gets a list of all job openings.
    /// </summary>
    /// <returns>
    ///     A list of job opening view models.
    /// </returns>
    List<JobOpeningViewModel> GetJobs();

    /// <summary>
    ///     Creates a new job opening.
    /// </summary>
    /// <param name="jobOpening">The job opening to create.</param>
    /// <param name="createdBy">The user who created the job opening.</param>
    /// <returns>
    ///     The log content and the new job opening's id.
    /// </returns>
    (LogContent, int) Create(JobOpeningViewModel jobOpening, string createdBy);

    /// <summary>
    ///     Gets a job opening by its ID.
    /// </summary>
    /// <param name="id">The ID of the job opening to get.</param>
    /// <returns>
    ///     A job opening view model, or null if no such job opening exists.
    /// </returns>
    JobOpeningViewModel GetById(int id);

    /// <summary>
    ///     Gets the by identifier clean.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    JobOpening GetByIdClean(int id);

    /// <summary>
    ///     Updates an existing job opening.
    /// </summary>
    /// <param name="jobOpening">The job opening to update.</param>
    /// <param name="updatedBy">The user who updated the job opening.</param>
    /// <returns></returns>
    LogContent Update(JobOpeningViewModel jobOpening, string updatedBy);

    /// <summary>
    ///     Deletes a job opening.
    /// </summary>
    /// <param name="jobOpening">The job opening to delete.</param>
    void Delete(JobOpeningViewModel jobOpening);

    /// <summary>
    ///     Gets all job opening ids.
    /// </summary>
    /// <returns></returns>
    List<int> GetAllJobOpeningIds();

    /// <summary>
    ///     Gets the jobs with related applications.
    /// </summary>
    /// <returns>
    ///     A list of JobOpeningViewModels.
    /// </returns>
    List<JobOpeningViewModel> GetJobsWithApplications();

    List<JobOpeningViewModel> GetJobsWithApplicationsSorted();

    /// <summary>
    ///     Gets the job opening title by its id.
    /// </summary>
    /// <param name="id">The job opening id.</param>
    /// <returns>
    ///     The job opening title.
    /// </returns>
    string GetJobOpeningTitleById(int id);

    /// <summary>
    ///     Gets the related user ids.
    /// </summary>
    /// <param name="jobOpeningId">The job opening id.</param>
    /// <returns>
    ///     A list of user ids.
    /// </returns>
    List<string> GetLinkedUserIds(int jobOpeningId);

    /// <summary>
    ///     Retrieves the list of user IDs associated with a specific job opening user.
    /// </summary>
    /// <param name="jobOpeningId">The ID of the job opening..</param>
    /// <returns>A list of integers. </returns>
    List<int> GetUserById(int jobOpeningId);

    /// <summary>
    ///     Updates the many-to-many relationship between User and JobOpening.
    /// </summary>
    /// <param name="jobOpeningId">The job opening id.</param>
    /// <param name="assignedUserIds">The assigned user ids.</param>
    void UpdateJobOpeningUsers(int jobOpeningId, List<string> assignedUserIds);
}
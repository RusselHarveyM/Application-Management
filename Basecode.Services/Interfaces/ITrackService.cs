using Basecode.Data.Models;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface ITrackService
    {
        /// <summary>
        /// Checks and sends application status based on resume evaluation.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="jobOpening">The job opening.</param>
        /// <returns></returns>
        Task<Application> CheckAndSendApplicationStatus(Application application, Applicant applicant, JobOpening jobOpening);

        /// <summary>
        /// Updates the track status email.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        /// <returns></returns>
        Task UpdateTrackStatusEmail(Application application, User user, string newStatus, string mailType);

        /// <summary>
        /// Updates the application status.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        /// <returns></returns>
        Task<Application> UpdateApplicationStatus(Application application, User user, string newStatus, string mailType);

        /// <summary>
        /// Updates the application status based on the response through email.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user</param>
        /// <param name="choice">The choice (e.g., approved or rejected).</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task<Application> UpdateApplicationStatusByEmailResponse(Application application, User user, string choice, string newStatus);

        /// <summary>
        /// Notifies the applicant and user about the application status.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task StatusNotification(Applicant applicant, User user, string newStatus);


        /// <summary>
        /// Notifies the applicant about the regret for not being shortlisted.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="job">The job position.</param>
        /// <returns></returns>
        Task RegretNotification(Applicant applicant, string job);
    }
}

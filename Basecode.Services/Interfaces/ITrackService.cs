using Basecode.Data.Models;

namespace Basecode.Services.Interfaces;

public interface ITrackService
{
    /// <summary>
    ///     Checks and sends application status based on resume evaluation.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="applicant">The applicant.</param>
    /// <param name="jobOpening">The job opening.</param>
    /// <returns></returns>
    Task<Application?> CheckAndSendApplicationStatus(Application application);

    /// <summary>
    ///     Updates the track status email.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="mailType">Type of the mail.</param>
    /// <returns></returns>
    void UpdateTrackStatusEmail(Application application, User user, string newStatus, string mailType, string oldStatus = "");

    /// <summary>
    ///     Updates the application status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="mailType">Type of the mail.</param>
    /// <returns></returns>
    Application UpdateApplicationStatus(Application application, User user, string newStatus, string mailType);

    /// <summary>
    ///     Updates the application status based on the response through email.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user</param>
    /// <param name="choice">The choice (e.g., approved or rejected).</param>
    /// <param name="newStatus">The new status.</param>
    /// <returns></returns>
    Application UpdateApplicationStatusByEmailResponse(Application application, User user, string choice,
        string newStatus);

    /// <summary>
    ///     Notifies the applicant and user about the application status.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    /// <returns></returns>
    void StatusNotification(Applicant applicant, User user, string newStatus);


    /// <summary>
    ///     Notifies the applicant about the regret for not being shortlisted.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="job">The job position.</param>
    /// <returns></returns>
    void RegretNotification(Applicant applicant, string job);

    /// <summary>
    ///     Sends gratitude notification.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="reference">The reference.</param>
    /// <returns></returns>
    void GratitudeNotification(Applicant applicant, BackgroundCheck reference);

    /// <summary>
    /// Sends the background check notification to HR and reference.
    /// </summary>
    /// <param name="reference">The reference.</param>
    /// <param name="user">The user.</param>
    /// <param name="applicant">The applicant.</param>
    /// <returns></returns>
    Task SendBackgroundCheckNotification(BackgroundCheck reference, User user, Applicant applicant);


    Application UpdateApplicationStatusByEmailResponseCurrentHires(Application application, User user,
        string choice, string status);
}
using Basecode.Data.Models;
using Basecode.Data.ViewModels;

namespace Basecode.Services.Interfaces;

public interface IDashboardService
{
    /// <summary>
    ///     Updates the status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="status">The status.</param>
    /// <param name="mailType"></param>
    /// <returns></returns>
    void UpdateStatus(Application application, User user, string status, string mailType);

    /// <summary>
    ///     Gets the shorlisted applicatons.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    List<Application> GetShorlistedApplicatons(string type, int jobId);


    /// <summary>
    /// Get Dashboard View Model
    /// </summary>
    /// <returns></returns>
    DashboardViewModel GetDashboardViewModel();

    /// <summary>
    /// Gets the directory view model for JobOpeningView view.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="jobId"></param>
    /// <returns></returns>
    ApplicantDirectoryViewModel GetApplicantDirectoryViewModel(string email, int jobId);

    /// <summary>
    /// Gets the directory view model for DirectoryView view.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="userAspId"></param>
    /// <returns></returns>
    Task<ApplicantDirectoryViewModel> GetDirectoryViewModel(string email, string userAspId);

    /// <summary>
    ///     Gets the application by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Application GetApplicationById(Guid id);

    void SendListEmail(int appId, string email, string name);

    /// <summary>
    /// Exports references' answers to pdf
    /// </summary>
    /// <param name="appId"></param>
    void ExportReferenceToPdf(string email, string name, int appId);
}
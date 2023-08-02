using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class DashboardService : IDashboardService
{
    private readonly IApplicationService _applicationService;

    private readonly ITrackService _trackService;

    public DashboardService(ITrackService trackService, IApplicationService applicationService)
    {
        _trackService = trackService;
        _applicationService = applicationService;
    }

    /// <summary>
    ///     Gets the shorlisted applicatons.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    public List<Application> GetShorlistedApplicatons(string type, int jobId)
    {
        return _applicationService.GetShorlistedApplicatons(type, jobId);
    }

    /// <summary>
    ///     Gets the application by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Application GetApplicationById(Guid id)
    {
        return _applicationService.GetApplicationById(id);
    }

    /// <summary>
    ///     Updates the status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="status">The status.</param>
    public void UpdateStatus(Application application, User user, string status)
    {
        var result = _trackService.UpdateApplicationStatus(application, user, status, null);
        if (result != null)
        {
            _applicationService.Update(result);
            _trackService.UpdateTrackStatusEmail(application, user, status, "Approval");
        }
    }
}
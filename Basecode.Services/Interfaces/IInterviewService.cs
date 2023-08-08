using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface IInterviewService
{
    /// <summary>
    ///     Adds the interview.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <returns></returns>
    LogContent AddInterview(UserSchedule schedule, string teamsLink);

    /// <summary>
    /// Updates the interview result.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    LogContent UpdateInterviewResult(Guid applicationId, string status, string choice);


    Application CheckInterview(Application application, string status, string choice);
}
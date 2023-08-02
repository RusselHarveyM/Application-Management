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
}
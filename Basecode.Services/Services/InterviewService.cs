using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class InterviewService : ErrorHandling, IInterviewService
{
    private readonly IInterviewRepository _repository;

    public InterviewService(IInterviewRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Adds the interview.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <returns></returns>
    public LogContent AddInterview(UserSchedule schedule, string teamsLink)
    {
        var logContent = CheckUserSchedule(schedule);

        if (logContent.Result == false)
        {
            var interview = new Interview
            {
                ApplicationId = schedule.ApplicationId,
                UserId = schedule.UserId,
                Date = schedule.Schedule,
                Type = schedule.Type,
                TeamsLink = teamsLink
            };

            _repository.AddInterview(interview);
        }

        return logContent;
    }
}
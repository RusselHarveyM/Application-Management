using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class InterviewService : ErrorHandling, IInterviewService
{
    private readonly IInterviewRepository _repository;
    private readonly IUserScheduleService _userScheduleService;
    private readonly List<string> _interviewStatuses;

    public InterviewService(IInterviewRepository repository, IUserScheduleService userScheduleService)
    {
        _repository = repository;
        _userScheduleService = userScheduleService;
        _interviewStatuses = new List<string>
        {
            "For HR Interview",
            "For Technical Interview",
            "For Final Interview",
        };
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

    /// <summary>
    /// Updates the interview result.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    public LogContent UpdateInterviewResult(Guid applicationId, string status, string choice)
    {
        var logContent = new LogContent();
        status = status.Replace("For ", "");

        var interview = _repository.GetInterviewByApplicationIdAndStatus(applicationId, status);

        if (interview != null)
        {
            var result = "Fail";
            if (choice == "approved") result = "Pass";

            interview.Result = result;

            logContent = CheckInterview(interview);
            if (!logContent.Result)
            {
                _repository.UpdateInterview(interview);
                var userSchedule = _userScheduleService.GetUserScheduleByApplicationId(applicationId);
                if (userSchedule != null)
                {
                    _userScheduleService.DeleteUserSchedule(userSchedule);
                }
            }
        }

        return logContent;
    }

    public Application CheckInterview(Application application, string status, string choice)
    {
        if (_interviewStatuses.Contains(status))
        {
            UpdateInterviewResult(application.Id, status, choice);
            if (status == "For Technical Interview" && choice.Equals("approved"))
                return application;
        }

        return null;
    }
}
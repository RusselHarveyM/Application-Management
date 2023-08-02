using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using NLog;

namespace Basecode.Services.Services;

public class UserScheduleService : ErrorHandling, IUserScheduleService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUserScheduleRepository _repository;

    public UserScheduleService(IUserScheduleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Gets the identifier if user schedule exists.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns>The user schedule identifier.</returns>
    public int GetIdIfUserScheduleExists(Guid applicationId)
    {
        return _repository.GetIdIfUserScheduleExists(applicationId);
    }

    /// <summary>
    ///     Adds a schedule in the UserSchedule table.
    /// </summary>
    /// <param name="userSchedule">The schedule.</param>
    /// <returns></returns>
    public (LogContent, int) AddUserSchedule(UserSchedule userSchedule)
    {
        var logContent = CheckUserSchedule(userSchedule);
        var userScheduleId = -1;

        if (logContent.Result == false) userScheduleId = _repository.AddUserSchedule(userSchedule);

        return (logContent, userScheduleId);
    }

    /// <summary>
    ///     Adds multiple UserSchedules.
    /// </summary>
    public void AddUserSchedules(List<UserSchedule> userSchedules)
    {
        _repository.AddUserSchedules(userSchedules);
    }

    /// <summary>
    ///     Gets the user schedule by identifier.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    /// <returns></returns>
    public UserSchedule GetUserScheduleById(int userScheduleId)
    {
        return _repository.GetUserScheduleById(userScheduleId);
    }

    /// <summary>
    ///     Updates the schedule.
    /// </summary>
    public LogContent UpdateUserSchedule(UserSchedule userSchedule, int? idToSetAsPending = null)
    {
        var logContent = CheckUserSchedule(userSchedule);

        if (logContent.Result == false)
        {
            var idToUpdate = idToSetAsPending ?? userSchedule.Id;

            var scheduleToBeUpdated = _repository.GetUserScheduleById(idToUpdate);
            scheduleToBeUpdated.Schedule = userSchedule.Schedule;
            scheduleToBeUpdated.Status = userSchedule.Status;
            _repository.UpdateUserSchedule(scheduleToBeUpdated);
        }

        return logContent;
    }

    /// <summary>
    ///     Deletes the user schedule.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    public void DeleteUserSchedule(UserSchedule userSchedule)
    {
        _repository.DeleteUserSchedule(userSchedule);
        _logger.Trace("UserSchedule record has been deleted.");
    }
}
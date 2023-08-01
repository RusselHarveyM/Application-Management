using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface ISchedulerService
    {
        /// <summary>
        /// Adds the schedules from the HR Scheduler.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        (LogContent, Dictionary<string, string>) AddSchedules(SchedulerDataViewModel formData, int userId);

        /// <summary>
        /// Checks the user schedule status.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        void CheckScheduleStatus(int userScheduleId);

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        Task<LogContent> AcceptSchedule(int userScheduleId);

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        LogContent RejectSchedule(int userScheduleId);

        /// <summary>
        /// Sets the online meeting schedule.
        /// </summary>
        Task<string> SetOnlineMeetingSchedule(UserSchedule userSchedule);
    }
}

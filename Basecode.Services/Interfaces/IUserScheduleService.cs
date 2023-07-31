using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IUserScheduleService
    {
        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        /// <returns></returns>
        (LogContent, int) AddUserSchedule(UserSchedule userSchedule);

        /// <summary>
        /// Adds multiple UserSchedules.
        /// </summary>
        void AddUserSchedules(List<UserSchedule> userSchedules);

        /// <summary>
        /// Gets the user schedule by identifier.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        UserSchedule GetUserScheduleById(int userScheduleId);

        /// <summary>
        /// Updates the schedule.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        /// <returns></returns>
        LogContent UpdateUserSchedule(UserSchedule userSchedule, int? idToSetAsPending = null);

        /// <summary>
        /// Gets the identifier if user schedule exists.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        int GetIdIfUserScheduleExists(Guid applicationId);

        /// <summary>
        /// Deletes the user schedule.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        void DeleteUserSchedule(UserSchedule userSchedule);
    }
}

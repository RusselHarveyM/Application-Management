using Basecode.Data.Models;

namespace Basecode.Data.Interfaces
{
    public interface IUserScheduleRepository
    {
        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        int AddUserSchedule(UserSchedule userSchedule);

        /// <summary>
        /// Inserts multiple UserSchedule records into the database.
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
        void UpdateUserSchedule(UserSchedule userSchedule);

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

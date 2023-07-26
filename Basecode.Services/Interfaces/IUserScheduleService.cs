﻿using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IUserScheduleService
    {
        /// <summary>
        /// Adds the user schedules.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <returns></returns>
        Task AddUserSchedules(SchedulerDataViewModel formData);

        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        /// <returns></returns>
        (LogContent, int) AddUserSchedule(UserSchedule userSchedule);

        /// <summary>
        /// Sends the schedule to applicant.
        /// </summary>
        Task SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, int applicantId, string meetingType);

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
        LogContent UpdateUserSchedule(UserSchedule userSchedule);

        /// <summary>
        /// Sends the schedules to interviewer.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        Task SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId, List<int> successfullyAddedApplicantIds);

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        LogContent AcceptSchedule(int userScheduleId);

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        Task<LogContent> RejectSchedule(int userScheduleId);

        /// <summary>
        /// Informs the interviewer that a schedule has been rejected.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        /// <returns></returns>
        Task SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule);
    }
}

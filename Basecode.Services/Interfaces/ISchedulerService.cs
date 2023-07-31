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
        /// Sends the schedule to applicant.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <param name="meetingType">Type of the meeting.</param>
        void SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, int applicantId, string meetingType);

        /// <summary>
        /// Checks the schedule status after token expiry.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        public void CheckScheduleStatusAfterTokenExpiry(int userScheduleId);

        /// <summary>
        /// Sends the schedules to interviewer.
        /// </summary>
        /// <param name="formData">The form data.</param>
        /// <param name="userId">The user identifier.</param>
        void SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId, List<int> successfullyAddedApplicantIds);

        /// <summary>
        /// Accepts the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        LogContent AcceptSchedule(int userScheduleId);

        /// <summary>
        /// Rejects the schedule.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        LogContent RejectSchedule(int userScheduleId);

        /// <summary>
        /// Informs the interviewer that a schedule has been rejected.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        void SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule);

        /// <summary>
        /// Sets the online meeting schedule.
        /// </summary>
        string SetOnlineMeetingSchedule(UserSchedule userSchedule);

        /// <summary>
        /// Sends the accepted schedule with Teams link to the interviewer.
        /// </summary>
        void SendAcceptedScheduleToInterviewer(UserSchedule userSchedule, string joinUrl);

        /// <summary>
        /// Sends the accepted schedule with Teams link to the applicant.
        /// </summary>
        void SendAcceptedScheduleToApplicant(UserSchedule userSchedule, string joinUrl);
    }
}

using Basecode.Data.Models;
using Basecode.Data.ViewModels;

namespace Basecode.Services.Interfaces;

public interface IScheduleSendingService
{
    /// <summary>
    ///     Sends the schedule to applicant.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    /// <param name="meetingType">Type of the meeting.</param>
    /// <param name="applicantId">The applicant identifier.</param>
    void SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, string meetingType,
        int tokenExpiry, int applicantId = -1);

    /// <summary>
    ///     Sends the schedules to interviewer.
    /// </summary>
    /// <param name="formData">The form data.</param>
    /// <param name="userId">The user identifier.</param>
    void SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId);

    /// <summary>
    ///     Sends the rejected schedule notice to interviewer.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    void SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule);

    /// <summary>
    ///     Sends the accepted schedule to interviewer.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="joinUrl">The join URL.</param>
    void SendAcceptedScheduleToInterviewer(UserSchedule userSchedule, string joinUrl);

    /// <summary>
    ///     Sends the accepted schedule to applicant.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="joinUrl">The join URL.</param>
    void SendAcceptedScheduleToApplicant(UserSchedule userSchedule, string joinUrl);

    /// <summary>
    /// Schedules the sending of the approval email.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    void ScheduleApprovalEmail(UserSchedule userSchedule, int hoursLeft);

    /// <summary>
    /// Schedules the sending of the exam score reminder email.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="hoursLeft">The hours left until the scheduled time.</param>
    void ScheduleExamScoreReminderEmail(UserSchedule userSchedule, int hoursLeft);
}

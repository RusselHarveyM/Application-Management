﻿using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IEmailSendingService
    {
        /// <summary>
        /// Sends the interview notification.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        /// <param name="role">The role</param>
        /// <returns></returns>
        Task SendInterviewNotification(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                   string interviewerPassword, string jobPosition, string role);

        /// <summary>
        /// Sends automated reminders to users based on their roles and assigned tasks.
        /// This asynchronous method will be executed periodically to remind users of their pending tasks.
        Task SendAutomatedReminder();

        /// <summary>
        /// Sends the unique identifier email.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        Task SendGUIDEmail(Application application);

        /// <summary>
        /// Sends the status notification.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task SendStatusNotification(User user, Applicant applicant, string newStatus);

        /// <summary>
        /// Sends the approval email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task SendApprovalEmail(User user, Applicant applicant, Guid appId, string newStatus);

        /// <summary>
        /// Sends the rejected email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task SendRejectedEmail(Applicant applicant, string newStatus);

        /// <summary>
        /// Sends the regret email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="job">The job.</param>
        /// <returns></returns>
        Task SendRegretEmail(Applicant applicant, string job);

        /// <summary>
        /// Sends the schedule to applicant.
        /// </summary>
        Task SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, Applicant applicant, string meetingType, int tokenExpiry);

        /// <summary>
        /// Sends the schedules to interviewer.
        /// </summary>
        Task SendSchedulesToInterviewer(string fullname, string userEmail, string jobOpeningTitle, DateOnly date, string scheduledTimes, string meetingType);

        /// <summary>
        /// Sends the gratitude email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        Task SendGratitudeEmail(Applicant applicant, BackgroundCheck reference);

        /// <summary>
        /// Informs the interviewer that a schedule has been rejected.
        /// </summary>
        /// <returns></returns>
        Task SendRejectedScheduleNoticeToInterviewer(string email, string fullname, UserSchedule userSchedule, string applicantFullName);

        /// <summary>
        /// Sends the accepted schedule with Teams link to the interviewer.
        /// </summary>
        Task SendAcceptedScheduleToInterviewer(string email, string fullname, UserSchedule userSchedule, ApplicationViewModel application, string joinUrl);

        /// <summary>
        /// Sends the accepted schedule with Teams link to the applicant.
        /// </summary>
        Task SendAcceptedScheduleToApplicant(string email, UserSchedule userSchedule, ApplicationViewModel application, string joinUrl);
    }
}

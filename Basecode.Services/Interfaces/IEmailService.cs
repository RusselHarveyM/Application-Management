using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends the notify email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task SendNotifyEmail(Applicant applicant, string newStatus);

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

        Task SendRegretEmail(Applicant applicant, string job);

        void ScheduleInterview(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);
        void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);
        void ScheduleForTechnical(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);
    }
}

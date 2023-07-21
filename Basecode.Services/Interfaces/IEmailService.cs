using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendNotifyEmail(Applicant applicant, string newStatus);

        Task SendStatusNotification(User user, Applicant applicant, string newStatus);

        Task SendApprovalEmail(User user, Applicant applicant, Guid appId, string newStatus);

        Task SendRejectedEmail(Applicant applicant, string newStatus);

        void ScheduleInterview(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);
       void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);
    }
}

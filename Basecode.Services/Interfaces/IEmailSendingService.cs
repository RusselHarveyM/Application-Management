using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IEmailSendingService
    {
        Task SendInterviewNotification(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                   string interviewerPassword, string jobPosition);

        Task SendInterviewNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                      string interviewerPassword, string jobPosition);

        Task SendHrNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition);

        Task SendTechnicalNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                      string interviewerPassword, string jobPosition);

        Task SendGUIDEmail(Application application);

        Task SendStatusNotification(User user, Applicant applicant, string newStatus);

        Task SendApprovalEmail(User user, Applicant applicant, Guid appId, string newStatus);

        Task SendRejectedEmail(Applicant applicant, string newStatus);

        Task SendRegretEmail(Applicant applicant, string job);
    }
}

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

        Task SendNotifyHREmail(Applicant applicant, string newStatus);

        Task SendApprovalEmail(User user, Applicant applicant);

        Task SendRejectedEmail(Applicant applicant, string newStatus);
    }
}

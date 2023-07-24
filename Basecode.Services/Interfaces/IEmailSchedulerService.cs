using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IEmailSchedulerService
    {
        void ScheduleInterview(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                           string interviewerPassword, string jobPosition);

        void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                           string interviewerPassword, string jobPosition);

        void ScheduleForTechnical(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                  string interviewerPassword, string jobPosition);
    }
}

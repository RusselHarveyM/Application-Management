using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IEmailSchedulerService
    {
        /// <summary>
        /// Schedules the interview.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        void ScheduleForDT(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                           string interviewerPassword, string jobPositione);

        /// <summary>
        /// Schedules for hr.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                           string interviewerPassword, string jobPosition);

        /// <summary>
        /// Schedules for technical.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        void ScheduleForTechnical(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                                  string interviewerPassword, string jobPosition);
    }
}

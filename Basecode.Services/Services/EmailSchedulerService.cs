using Basecode.Services.Interfaces;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class EmailSchedulerService : IEmailSchedulerService
    {
        private readonly IEmailSendingService _emailSendingService;

        public EmailSchedulerService(IEmailSendingService emailSendingService) 
        {
            _emailSendingService = emailSendingService;
        }

        /// <summary>
        /// Schedules the interview.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        public void ScheduleDT(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                              string interviewerPassword, string jobPosition)
        {
            // Schedule the email notification using Hangfire
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds
        }

        /// <summary>
        /// Schedules for hr.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        public void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds
        }

        /// <summary>
        /// Schedules for technical.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
        public void ScheduleForTechnical(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds
        }
    }
}

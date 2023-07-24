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

        public void ScheduleInterview(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                              string interviewerPassword, string jobPosition)
        {
            // Schedule the email notification using Hangfire
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds

            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotif2weeks(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromDays(14)); // Delay of 2 weeks
        }

        public void ScheduleForHR(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds

            BackgroundJob.Schedule(() => _emailSendingService.SendHrNotif2weeks(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromDays(14)); // Delay of 2 weeks
        }

        public void ScheduleForTechnical(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {
            BackgroundJob.Schedule(() => _emailSendingService.SendInterviewNotification(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromSeconds(5)); // Delay of 5 seconds

            BackgroundJob.Schedule(() => _emailSendingService.SendTechnicalNotif2weeks(interviewerEmail, intervierwerFullName, interviewerUsername,
                                                                   interviewerPassword, jobPosition),
                                                                   TimeSpan.FromDays(14)); // Delay of 2 weeks
        }
    }
}

using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.SqlServer.Server;
using NLog;

namespace Basecode.Services.Services
{
    public class UserScheduleService : ErrorHandling, IUserScheduleService
    {
        private readonly IUserScheduleRepository _repository;
        private readonly IEmailService _emailService;
        private readonly IApplicationService _applicationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserScheduleService(IUserScheduleRepository repository, IEmailService emailService, IApplicationService applicationService)
        {
            _repository = repository;
            _emailService = emailService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// Creates records in the UserSchedule table.
        /// </summary>
        /// <param name="formData">The HR Scheduler form data.</param>
        public void CreateSchedules(SchedulerDataViewModel formData)
        {
            // Create a new UserSchedule record for each applicant
            foreach (var schedule in formData.ApplicantSchedules)
            {
                Guid applicationId = _applicationService.GetApplicationIdByApplicantId(schedule.ApplicantId);

                var userSchedule = new UserSchedule
                {
                    UserId = 1, // Temporary until auth has been sorted out
                    Type = formData.Type,
                    Schedule = DateTime.Parse(formData.Date.ToString() + " " + schedule.Time),
                    ApplicationId = applicationId
                };

                var data = CreateSchedule(userSchedule);

                if (!data.Result)
                {
                    _logger.Trace("Successfully created a new UserSchedule record.");
                }
            }
        }

        public LogContent CreateSchedule(UserSchedule userSchedule)
        {
            LogContent logContent = new LogContent();
            // Still need to add checking in ErrorHandling

            if (logContent.Result == false)
            {
                _repository.Create(userSchedule);
            }

            return logContent;
        }
    }
}

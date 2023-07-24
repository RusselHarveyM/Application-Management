﻿using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class InterviewService : ErrorHandling, IInterviewService
    {
        private readonly IInterviewRepository _repository;

        public InterviewService(IInterviewRepository repository)
        {
            _repository = repository;
        }

        public LogContent AddInterview(UserSchedule schedule)
        {
            LogContent logContent = new LogContent();
            // error handling TBA

            if (logContent.Result == false)
            {
                var interview = new Interview
                {
                    ApplicationId = schedule.ApplicationId,
                    UserId = schedule.UserId,
                    Date = schedule.Schedule,
                    Type = schedule.Type,
                };

                _repository.AddInterview(interview);
            }

            return logContent;
        }
    }
}
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class UserScheduleService : ErrorHandling, IUserScheduleService
    {
        private readonly IUserScheduleRepository _repository;

        public UserScheduleService(IUserScheduleRepository repository)
        {
            _repository = repository;
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

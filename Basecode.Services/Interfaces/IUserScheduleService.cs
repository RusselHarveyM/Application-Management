using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IUserScheduleService
    {
        void CreateSchedules(SchedulerDataViewModel formData);

        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        /// <returns></returns>
        LogContent CreateSchedule(UserSchedule userSchedule);
    }
}

using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IUserScheduleService
    {
        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        /// <returns></returns>
        LogContent CreateSchedule(UserSchedule userSchedule);
    }
}

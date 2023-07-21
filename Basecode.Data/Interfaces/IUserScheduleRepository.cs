using Basecode.Data.Models;

namespace Basecode.Data.Interfaces
{
    public interface IUserScheduleRepository
    {
        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        void Create(UserSchedule userSchedule);
    }
}

using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories
{
    public class UserScheduleRepository : BaseRepository, IUserScheduleRepository
    {
        private readonly BasecodeContext _context;

        public UserScheduleRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a UserSchedule.
        /// </summary>
        /// <param name="userSchedule"></param>
        /// <returns></returns>
        public int AddUserSchedule(UserSchedule userSchedule)
        {
            _context.UserSchedule.Add(userSchedule);
            _context.SaveChanges();
            return userSchedule.Id;
        }

        /// <summary>
        /// Gets the user schedule by identifier.
        /// </summary>
        /// <param name="userScheduleId">The user schedule identifier.</param>
        /// <returns></returns>
        public UserSchedule GetUserScheduleById(int userScheduleId)
        {
            return _context.UserSchedule.Find(userScheduleId);
        }

        /// <summary>
        /// Updates the specified user schedule.
        /// </summary>
        /// <param name="userSchedule">The user schedule.</param>
        public void UpdateUserSchedule(UserSchedule userSchedule)
        {
            _context.UserSchedule.Update(userSchedule);
            _context.SaveChanges();
        }
    }
}

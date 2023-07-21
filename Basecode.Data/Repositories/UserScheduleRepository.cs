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

        public void Create(UserSchedule userSchedule)
        {
            _context.UserSchedule.Add(userSchedule);
            _context.SaveChanges();
        }
    }
}

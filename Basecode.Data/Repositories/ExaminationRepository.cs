using Basecode.Data.Interfaces;

namespace Basecode.Data.Repositories
{
    public class ExaminationRepository : BaseRepository, IExaminationRepository
    {
        private readonly BasecodeContext _context;

        public ExaminationRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }
    }
}

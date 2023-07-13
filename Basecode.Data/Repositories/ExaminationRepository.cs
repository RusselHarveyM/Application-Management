using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories
{
    public class ExaminationRepository : BaseRepository, IExaminationRepository
    {
        private readonly BasecodeContext _context;

        public ExaminationRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public IQueryable<Examination> GetAllExaminations()
        {
            return this.GetDbSet<Examination>();
        }
    }
}

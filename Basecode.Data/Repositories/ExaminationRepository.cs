using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories;

public class ExaminationRepository : BaseRepository, IExaminationRepository
{
    private readonly BasecodeContext _context;

    public ExaminationRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    /// <summary>
    ///     Gets the examinations by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    public IQueryable<Examination> GetExaminationsByJobOpeningId(int jobOpeningId)
    {
        return GetDbSet<Examination>()
            .Where(exam => exam.Application.JobOpeningId == jobOpeningId);
    }

    /// <summary>
    ///     Adds the examination.
    /// </summary>
    /// <param name="examination">The examination.</param>
    public void AddExamination(Examination examination)
    {
        _context.Examination.Add(examination);
        _context.SaveChanges();
    }
}
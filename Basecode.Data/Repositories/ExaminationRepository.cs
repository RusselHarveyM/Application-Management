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
    public IQueryable<Examination> GetShortlistableExamsByJobOpeningId(int jobOpeningId)
    {
        return GetDbSet<Examination>()
            .Where(exam => exam.Application.JobOpeningId == jobOpeningId)
            .Where(exam => exam.Application.Status == "For Technical Interview")
            .Where(exam => exam.Application.Interviews
                .Any(interview => interview.Type == "Technical Interview" && interview.Result == "Pass"));
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

    /// <summary>
    /// Gets the examination by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    public Examination GetExaminationByApplicationId(Guid applicationId)
    {
        return GetDbSet<Examination>()
            .Where(exam => exam.ApplicationId == applicationId).FirstOrDefault();
    }

    /// <summary>
    /// Gets the examination by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Examination GetExaminationById(int examinationId)
    {
        return _context.Examination.Find(examinationId);
    }

    public void UpdateExamination(Examination examination)
    {
        _context.Examination.Update(examination);
        _context.SaveChanges();
    }
}
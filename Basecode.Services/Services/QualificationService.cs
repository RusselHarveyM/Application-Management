using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class QualificationService : IQualificationService
{
    private readonly IQualificationRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="QualificationService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public QualificationService(IQualificationRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Gets the qualifications.
    /// </summary>
    /// <returns></returns>
    public List<Qualification> GetQualifications()
    {
        var data = _repository.GetAll()
            .Select(m => new Qualification
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .ToList();

        return data;
    }

    /// <summary>
    ///     Creates the specified qualification.
    /// </summary>
    /// <param name="qualification">The qualification.</param>
    public void Create(Qualification qualification)
    {
        _repository.AddQualification(qualification);
    }

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Qualification GetById(int id)
    {
        var data = _repository.GetAll()
            .Where(m => m.Id == id)
            .Select(m => new Qualification
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .FirstOrDefault();

        return data;
    }

    /// <summary>
    ///     Gets the qualifications by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    public List<Qualification> GetQualificationsByJobOpeningId(int jobOpeningId)
    {
        var data = _repository.GetAll()
            .Where(m => m.JobOpeningId == jobOpeningId)
            .Select(m => new Qualification
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .ToList();

        return data;
    }

    /// <summary>
    ///     Updates the specified qualification.
    /// </summary>
    /// <param name="qualification">The qualification.</param>
    public void Update(Qualification qualification)
    {
        var qualificationExisting = _repository.GetQualificationById(qualification.Id);
        qualificationExisting.Description = qualification.Description;

        _repository.UpdateQualification(qualificationExisting);
    }

    /// <summary>
    ///     Deletes the specified qualification.
    /// </summary>
    /// <param name="qualification">The qualification.</param>
    public void Delete(Qualification qualification)
    {
        _repository.DeleteQualification(qualification);
    }

    /// <summary>
    ///     Deletes the qualifications by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    public void DeleteQualificationsByJobOpeningId(int jobOpeningId)
    {
        var qualifications = _repository.GetAll()
            .Where(m => m.JobOpeningId == jobOpeningId)
            .ToList();

        foreach (var qualification in qualifications) _repository.DeleteQualification(qualification);
    }
}
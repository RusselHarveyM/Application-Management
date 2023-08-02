using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class ResponsibilityService : IResponsibilityService
{
    private readonly IResponsibilityRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ResponsibilityService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public ResponsibilityService(IResponsibilityRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Gets the responsibilities.
    /// </summary>
    /// <returns></returns>
    public List<Responsibility> GetResponsibilities()
    {
        var data = _repository.GetAll()
            .Select(m => new Responsibility
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .ToList();

        return data;
    }

    /// <summary>
    ///     Creates the specified responsibility.
    /// </summary>
    /// <param name="Responsibility"></param>
    public void Create(Responsibility Responsibility)
    {
        _repository.AddResponsibility(Responsibility);
    }

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Responsibility GetById(int id)
    {
        var data = _repository.GetAll()
            .Where(m => m.Id == id)
            .Select(m => new Responsibility
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .FirstOrDefault();

        return data;
    }

    /// <summary>
    ///     Gets the responsibilities by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    public List<Responsibility> GetResponsibilitiesByJobOpeningId(int jobOpeningId)
    {
        var data = _repository.GetAll()
            .Where(m => m.JobOpeningId == jobOpeningId)
            .Select(m => new Responsibility
            {
                Id = m.Id,
                JobOpeningId = m.JobOpeningId,
                Description = m.Description
            })
            .ToList();

        return data;
    }

    /// <summary>
    ///     Updates the specified responsibility.
    /// </summary>
    /// <param name="responsibility">The responsibility.</param>
    public void Update(Responsibility responsibility)
    {
        var responsibilityExisting = _repository.GetResponsibilityById(responsibility.Id);
        responsibilityExisting.Description = responsibility.Description;

        _repository.UpdateResponsibility(responsibilityExisting);
    }

    /// <summary>
    ///     Deletes the specified responsibility.
    /// </summary>
    /// <param name="responsibility">The responsibility.</param>
    public void Delete(Responsibility responsibility)
    {
        _repository.DeleteResponsibility(responsibility);
    }

    /// <summary>
    ///     Deletes the responsibilities by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    public void DeleteResponsibilitiesByJobOpeningId(int jobOpeningId)
    {
        var responsibilities = _repository.GetAll()
            .Where(m => m.JobOpeningId == jobOpeningId)
            .ToList();

        foreach (var responsibility in responsibilities) _repository.DeleteResponsibility(responsibility);
    }
}
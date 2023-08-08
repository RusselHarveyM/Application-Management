using Basecode.Data.Models;

namespace Basecode.Services.Interfaces;

public interface IResponsibilityService
{
    /// <summary>
    ///     Gets the responsibilities.
    /// </summary>
    /// <returns></returns>
    List<Responsibility> GetResponsibilities();

    /// <summary>
    ///     Creates the specified responsibility.
    /// </summary>
    /// <param name="responsibility">The responsibility.</param>
    void Create(Responsibility responsibility);

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Responsibility GetById(int id);

    /// <summary>
    ///     Gets the responsibilities by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    List<Responsibility> GetResponsibilitiesByJobOpeningId(int jobOpeningId);

    /// <summary>
    ///     Updates the specified responsibility.
    /// </summary>
    /// <param name="responsibility">The responsibility.</param>
    void Update(Responsibility responsibility);

    /// <summary>
    ///     Deletes the specified responsibility.
    /// </summary>
    /// <param name="responsibility">The responsibility.</param>
    void Delete(Responsibility responsibility);

    /// <summary>Deletes the responsibilities by job opening identifier.</summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    void DeleteResponsibilitiesByJobOpeningId(int jobOpeningId);
}
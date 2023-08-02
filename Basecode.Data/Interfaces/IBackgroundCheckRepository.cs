using Basecode.Data.Models;

namespace Basecode.Data.Interfaces;

public interface IBackgroundCheckRepository
{
    /// <summary>
    ///     Creates the specified form.
    /// </summary>
    /// <param name="form">The form.</param>
    /// <returns>Id of the created background check.</returns>
    int Create(BackgroundCheck form);

    /// <summary>
    ///     Gets all.
    /// </summary>
    /// <returns></returns>
    IQueryable<BackgroundCheck> GetAll();

    /// <summary>
    ///     Gets the by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    BackgroundCheck GetById(int id);
}
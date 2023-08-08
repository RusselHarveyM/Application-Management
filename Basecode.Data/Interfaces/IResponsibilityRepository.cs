using Basecode.Data.Models;

namespace Basecode.Data.Interfaces;

/// <summary>
///     Interface for the Responsibility repository.
/// </summary>
public interface IResponsibilityRepository
{
    /// <summary>
    ///     Gets all Responsibilitys.
    /// </summary>
    /// <returns>An IQueryable of Responsibility objects.</returns>
    IQueryable<Responsibility> GetAll();

    /// <summary>
    ///     Adds a new Responsibility.
    /// </summary>
    /// <param name="responsibility">The Responsibility to add.</param>
    void AddResponsibility(Responsibility responsibility);

    /// <summary>
    ///     Gets a Responsibility by its ID.
    /// </summary>
    /// <param name="id">The ID of the Responsibility to get.</param>
    /// <returns>The Responsibility object with the specified ID.</returns>
    Responsibility GetResponsibilityById(int id);

    /// <summary>
    ///     Updates an existing Responsibility.
    /// </summary>
    /// <param name="responsibility">The Responsibility to update.</param>
    void UpdateResponsibility(Responsibility responsibility);

    /// <summary>
    ///     Deletes a Responsibility.
    /// </summary>
    /// <param name="responsibility">The Responsibility to delete.</param>
    void DeleteResponsibility(Responsibility responsibility);
}
using Basecode.Data.Models;

namespace Basecode.Data.Interfaces;

public interface IExaminationRepository
{
    /// <summary>
    ///     Gets the examinations by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    IQueryable<Examination> GetShortlistableExamsByJobOpeningId(int jobOpeningId);

    /// <summary>
    ///     Adds the examination.
    /// </summary>
    /// <param name="examination">The examination.</param>
    void AddExamination(Examination examination);

    /// <summary>
    /// Gets the examination by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    Examination GetExaminationByApplicationId(Guid applicationId);

    /// <summary>
    /// Gets the examination by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Examination GetExaminationById(int examinationId);

    /// <summary>
    /// Updates the examination.
    /// </summary>
    /// <param name="examination">The examination.</param>
    void UpdateExamination(Examination examination);
}
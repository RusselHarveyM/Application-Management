using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface IExaminationService
{
    /// <summary>
    ///     Gets the examinations by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    List<Examination> GetShortlistableExamsByJobOpeningId(int jobOpeningId);

    /// <summary>
    ///     Adds the examination.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <returns></returns>
    LogContent AddExamination(UserSchedule schedule, string teamsLink);

    /// <summary>
    /// Gets the examination by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    Examination GetExaminationByApplicationId(Guid applicationId);

    /// <summary>
    /// Updates the examination score.
    /// </summary>
    /// <param name="examinationId">The examination identifier.</param>
    /// <param name="score">The score.</param>
    /// <returns></returns>
    LogContent UpdateExaminationScore(int examinationId, int score);

    /// <summary>
    /// Updates the application status by exam result.
    /// </summary>
    /// <param name="examination">The examination.</param>
    /// <param name="result">The result.</param>
    void UpdateApplicationStatusByExamResult(Examination examination, string result);
}
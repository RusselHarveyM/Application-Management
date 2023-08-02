using Basecode.Data.Models;

namespace Basecode.Services.Interfaces;

public interface IShortlistingService
{
    /// <summary>
    ///     Shortlists the applications.
    /// </summary>
    void ShortlistApplications();

    /// <summary>
    ///     Gets the shortlist percentage.
    /// </summary>
    /// <param name="totalExams">The total exams.</param>
    /// <returns></returns>
    double GetShortlistPercentage(int totalExams);

    /// <summary>
    /// Updates the shortlisted applications.
    /// </summary>
    /// <param name="shortlistedExams">The shortlisted exams.</param>
    void UpdateShortlistedApplications(List<Examination> shortlistedExams);

    /// <summary>
    /// Updates the non shortlisted applications.
    /// </summary>
    /// <param name="nonShortlistedExams">The non shortlisted exams.</param>
    void UpdateNonShortlistedApplications(List<Examination> nonShortlistedExams);
}
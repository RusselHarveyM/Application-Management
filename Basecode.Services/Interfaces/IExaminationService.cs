using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IExaminationService
    {
        /// <summary>
        /// Gets the examinations by job opening identifier.
        /// </summary>
        /// <param name="jobOpeningId">The job opening identifier.</param>
        /// <returns></returns>
        List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId);

        /// <summary>
        /// Adds the examination.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <returns></returns>
        LogContent AddExamination(UserSchedule schedule, string teamsLink);
    }
}

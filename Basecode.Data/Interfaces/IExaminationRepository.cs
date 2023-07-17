using Basecode.Data.Models;

namespace Basecode.Data.Interfaces
{
    public interface IExaminationRepository
    {
        /// <summary>
        /// Gets the examinations by job opening identifier.
        /// </summary>
        /// <param name="jobOpeningId">The job opening identifier.</param>
        /// <returns></returns>
        IQueryable<Examination> GetExaminationsByJobOpeningId(int jobOpeningId);
    }
}

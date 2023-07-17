using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IExaminationService
    {
        /// <summary>
        /// Gets the examinations by the job opening ID
        /// </summary>
        /// <param name="jobOpeningId">The job opening ID.</param>
        /// <returns></returns>
        List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId);
    }
}

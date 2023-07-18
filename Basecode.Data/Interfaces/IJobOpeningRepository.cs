using Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface IJobOpeningRepository
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IQueryable<JobOpening> GetAll();
        /// <summary>
        /// Adds the job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening.</param>
        void AddJobOpening(JobOpening jobOpening);

        /// <summary>
        /// Gets the job opening by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        JobOpening GetJobOpeningById(int id);

        /// <summary>
        /// Updates the job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening.</param>
        void UpdateJobOpening(JobOpening jobOpening);

        /// <summary>
        /// Deletes the job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening.</param>
        void DeleteJobOpening(JobOpening jobOpening);

        /// <summary>
        /// Gets all job opening ids.
        /// </summary>
        /// <returns></returns>
        List<int> GetAllJobOpeningIds();
    }
}

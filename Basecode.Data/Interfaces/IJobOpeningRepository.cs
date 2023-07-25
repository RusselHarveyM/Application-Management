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
        /// <param name="jobOpening"></param>
        /// <returns>The job opening id.</returns>
        int AddJobOpening(JobOpening jobOpening);

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

        /// <summary>
        /// Gets the jobs with applications.
        /// </summary>
        /// <returns></returns>
        IQueryable<JobOpening> GetJobsWithApplications();

        /// <summary>
        /// Gets the job opening title by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        string GetJobOpeningTitleById(int id);

        /// <summary>
        /// Gets the linked user ids.
        /// </summary>
        /// <param name="jobOpeningId">The job opening identifier.</param>
        /// <returns></returns>
        IQueryable<int> GetLinkedUserIds(int jobOpeningId);

        /// <summary>
        /// Updates the job opening users.
        /// </summary>
        /// <param name="jobOpeningId">The job opening identifier.</param>
        /// <param name="assignedUserIds">The assigned user ids.</param>
        void UpdateJobOpeningUsers(int jobOpeningId, List<int> assignedUserIds);
    }
}

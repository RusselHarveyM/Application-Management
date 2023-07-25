using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IInterviewRepository
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        IQueryable<Interview> GetAll();
        /// <summary>
        /// Adds the interview.
        /// </summary>
        /// <param name="interview">The interview.</param>
        void AddInterview(Interview interview);
        /// <summary>
        /// Gets the interview by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Interview GetInterviewById(int id);
        /// <summary>
        /// Updates the interview.
        /// </summary>
        /// <param name="interview">The interview.</param>
        void UpdateInterview(Interview interview);
        /// <summary>
        /// Deletes the interview.
        /// </summary>
        /// <param name="interview">The interview.</param>
        void DeleteInterview(Interview interview);
    }
}

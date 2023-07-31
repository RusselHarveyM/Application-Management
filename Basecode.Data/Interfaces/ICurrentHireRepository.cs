using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface ICurrentHireRepository
    {
        /// <summary>
        /// Get current hire by Id
        /// </summary>
        /// <param name="currentHireId"></param>
        /// <returns></returns>
        CurrentHire GetCurrentHireById(int currentHireId);

        /// <summary>
        /// Get current hire id by user id
        /// </summary>
        /// <param name="currentHireId"></param>
        /// <returns></returns>
        int GetCurrentHireIdByUserId(int currentHireId);

        /// <summary>
        /// Get all current hire
        /// </summary>
        /// <returns></returns>
        IQueryable<CurrentHire> GetAll();

        /// <summary>
        /// Update current hire
        /// </summary>
        /// <param name="currentHire"></param>
        void UpdateCurrentHire(CurrentHire currentHire);

        /// <summary>
        /// Update current hire
        /// </summary>
        /// <param name="userOffer"></param>
        //void UpdateCurrentHires(CurrentHire currentHire);

        /// <summary>
        /// Add current hire
        /// </summary>
        /// <param name="currentHire"></param>
        /// <returns></returns>
        int AddCurrentHire(CurrentHire currenHire);

        /// <summary>
        /// Get id if current hire exists
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        int GetIdIfCurrentHireExists(Guid applicationId);
    }
}

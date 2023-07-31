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
        /// Get user offer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        CurrentHire GetUserOfferById(int userOfferId);

        /// <summary>
        /// Get user offer id by user id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        int GetCurrentHireIdByUserId(int userOfferId);

        /// <summary>
        /// Get all useroffer
        /// </summary>
        /// <returns></returns>
        IQueryable<CurrentHire> GetAll();
    }
}

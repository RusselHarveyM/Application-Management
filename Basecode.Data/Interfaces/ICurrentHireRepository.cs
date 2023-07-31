using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface IUserOfferRepository
    {
        /// <summary>
        /// Get user offer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        CurrentHire GetUserOfferById(int userOfferId);
    }
}

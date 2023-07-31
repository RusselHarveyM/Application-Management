using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface ICurrentHireService
    {
        /// <summary>
        /// Get UserOffer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        CurrentHire GetCurrentHireById(int currenHireId);
    }
}

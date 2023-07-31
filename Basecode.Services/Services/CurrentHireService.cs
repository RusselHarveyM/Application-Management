using NLog;
using static Basecode.Services.Services.ErrorHandling;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class CurrenHireService : ErrorHandling, ICurrentHireService
    {
        private readonly ICurrentHireRepository _currenHireRepository;

        public CurrenHireService(ICurrentHireRepository currenHireRepository)
        {
            _currenHireRepository = currenHireRepository;
        }

        /// <summary>
        /// Get UserOffer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public CurrentHire GetCurrentHireById(int currenHireId)
        {
            return _currenHireRepository.GetCurrentHireById(currenHireId);
        }
    }
}

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
using Basecode.Data.Repositories;

namespace Basecode.Services.Services
{
    public class CurrenHireService : ErrorHandling, ICurrentHireService
    {
        private readonly ICurrentHireRepository _currenHireRepository;
        private readonly IUserScheduleRepository _userScheduleRepository;

        public CurrenHireService(ICurrentHireRepository currenHireRepository, IUserScheduleRepository userScheduleRepository)
        {
            _currenHireRepository = currenHireRepository;
            _userScheduleRepository = userScheduleRepository;
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

        /// <summary>
        /// AcceptOffer and return UserOffer Status
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public LogContent AcceptOffer(int currentHireId)
        {
            var currentHire = GetCurrentHireById(currentHireId);
            LogContent logContent = CheckCurrentHireStatus(currentHire);

            if (logContent.Result == false)
            {
                currentHire.Status = "confirm";
                logContent = UpdateCurrentHire(currentHire);
            }

            return logContent;
        }

        /// <summary>
        /// Get UserSchedule by Id
        /// </summary>
        /// <param name="userScheduleId"></param>
        /// <returns></returns>
        public UserSchedule GetUserScheduleById(int userScheduleId)
        {
            return _userScheduleRepository.GetUserScheduleById(userScheduleId);
        }
    }
}

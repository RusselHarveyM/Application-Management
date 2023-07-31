﻿using Basecode.Data.Models;
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

        /// <summary>
        /// AcceptOffer and return UserOffer Status
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        LogContent AcceptOffer(int currentHireId);

        /// <summary>
        /// Get UserSchedule by Id
        /// </summary>
        /// <param name="userScheduleId"></param>
        /// <returns></returns>
        UserSchedule GetUserScheduleById(int userScheduleId);
    }
}

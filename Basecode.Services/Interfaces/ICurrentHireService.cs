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

        /// <summary>
        /// Reject offer and return UserOffer Status
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        Task<LogContent> RejectOffer(int currentHireId);

        /// <summary>
        /// Send reject notice to interviewer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <returns></returns>
        Task SendRejectedHireNoticeToInterviewer(CurrentHire currentHire);

        /// <summary>
        /// update user offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="idToSetAsPending"></param>
        /// <returns></returns>
        LogContent UpdateCurrentHire(CurrentHire currentHire, int? idToSetAsPending = null);

        /// <summary>
        /// Get Id if user offer existed
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        int GetIdIfCurrentHireExists(Guid applicationId);

        /// <summary>
        /// Add useroffer
        /// </summary>
        /// <param name="applicant"></param>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        Task AddCurrentHire(Applicant applicant, int currentHireId);

        /// <summary>
        /// Handle existing offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="existingId"></param>
        /// <param name="applicantId"></param>
        /// <param name="successfullyAddedApplicantIds"></param>
        /// <returns></returns>
        Task<List<int>> HandleExistingHire(CurrentHire currentHire, int existingId, int applicantId, List<int> successfullyAddedApplicantIds);

        /// <summary>
        /// Send offer to applicant
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="userOfferId"></param>
        /// <param name="applicantId"></param>
        /// <returns></returns>
        Task SendHireToApplicant(CurrentHire currentHire, int currentHireId, int applicantId);

        /// <summary>
        /// Handle new offer
        /// </summary>
        /// <param name="userOffer"></param>
        /// <param name="applicantId"></param>
        /// <param name="successfullyAddedApplicantIds"></param>
        /// <returns></returns>
        Task<List<int>> HandleNewHire(CurrentHire currentHire, int applicantId, List<int> successfullyAddedApplicantIds);
    }
}

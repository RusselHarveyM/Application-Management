using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface ICurrentHireService
{
    /// <summary>
    ///     Get current hire by Id
    /// </summary>
    /// <param name="currenHireId"></param>
    /// <returns></returns>
    CurrentHire GetCurrentHireById(int currenHireId);

    /// <summary>
    ///     AcceptOffer and return current hire Status
    /// </summary>
    /// <param name="currentHireId"></param>
    /// <returns></returns>
    LogContent AcceptOffer(int currentHireId);

    /// <summary>
    ///     Get UserSchedule by Id
    /// </summary>
    /// <param name="userScheduleId"></param>
    /// <returns></returns>
    UserSchedule GetUserScheduleById(int userScheduleId);

    /// <summary>
    ///     Reject offer and return current hire Status
    /// </summary>
    /// <param name="currentHireId"></param>
    /// <returns></returns>
    Task<LogContent> RejectOffer(int currentHireId);

    /// <summary>
    ///     Send reject notice to interviewer
    /// </summary>
    /// <param name="currentHire"></param>
    /// <returns></returns>
    Task SendRejectedHireNoticeToInterviewer(CurrentHire currentHire);

    /// <summary>
    ///     update current hire
    /// </summary>
    /// <param name="currentHire"></param>
    /// <param name="idToSetAsPending"></param>
    /// <returns></returns>
    LogContent UpdateCurrentHire(CurrentHire currentHire, int? idToSetAsPending = null);

    /// <summary>
    ///     Get Id if current hire existed
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    int GetIdIfCurrentHireExists(Guid applicationId);

    /// <summary>
    ///     Add current hire
    /// </summary>
    /// <param name="applicant"></param>
    /// <param name="currentHireId"></param>
    /// <returns></returns>
    Task AddCurrentHire(Applicant applicant, int currentHireId);

    /// <summary>
    ///     Handle existing hire
    /// </summary>
    /// <param name="currentHire"></param>
    /// <param name="existingId"></param>
    /// <param name="applicantId"></param>
    /// <param name="successfullyAddedApplicantIds"></param>
    /// <returns></returns>
    Task<List<int>> HandleExistingHire(CurrentHire currentHire, int existingId, int applicantId,
        List<int> successfullyAddedApplicantIds);

    /// <summary>
    ///     Send offer to applicant
    /// </summary>
    /// <param name="currentHire"></param>
    /// <param name="currentHireId"></param>
    /// <param name="applicantId"></param>
    /// <returns></returns>
    Task SendHireToApplicant(CurrentHire currentHire, int currentHireId, int applicantId);

    /// <summary>
    ///     Handle new hire
    /// </summary>
    /// <param name="currentHire"></param>
    /// <param name="applicantId"></param>
    /// <param name="successfullyAddedApplicantIds"></param>
    /// <returns></returns>
    Task<List<int>> HandleNewHire(CurrentHire currentHire, int applicantId, List<int> successfullyAddedApplicantIds);

    /// <summary>
    ///     Add current hire
    /// </summary>
    /// <param name="currentHire"></param>
    /// <returns></returns>
    (LogContent, int) AddCurrentHires(CurrentHire currentHire);
}
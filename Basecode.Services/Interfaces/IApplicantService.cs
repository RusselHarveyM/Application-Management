﻿using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces;

public interface IApplicantService
{
    /// <summary>
    ///     Retrieves a list of all applicants.
    /// </summary>
    /// <returns>
    ///     A list of Applicant objects.
    /// </returns>
    List<Applicant> GetApplicants();

    /// <summary>
    ///     Retrieves an applicant by its ID.
    /// </summary>
    /// <param name="id">The ID of the applicant.</param>
    /// <returns>
    ///     The Applicant object.
    /// </returns>
    Applicant GetApplicantById(int id);

    /// <summary>
    ///     Gets the applicant by identifier all.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    Applicant GetApplicantByIdAll(int id);

    /// <summary>
    ///     Updates the application.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="choice">The choice.</param>
    /// <param name="newStatus">The new status.</param>
    /// <returns></returns>
    void UpdateApplication(Application application, User user, string choice, string newStatus);

    /// <summary>
    ///     Creates a new applicant based on the provided applicant data.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <returns>
    ///     Returns a tuple with the log content and the ID of the created applicant.
    /// </returns>
    (LogContent, int) Create(ApplicantViewModel applicant);

    /// <summary>
    ///     Checks and sends the application status.
    /// </summary>
    Task CheckAndSendApplicationStatus(Guid applicationId);

    /// <summary>
    /// Gets the applicants by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <returns></returns>
    List<Applicant> GetApplicantsByJobOpeningId(int jobOpeningId);

    /// <summary>
    ///     Gets the applicants with rejected or no schedule.
    /// </summary>
    /// <returns></returns>
    List<ApplicantStatusViewModel> GetApplicantsWithRejectedOrNoSchedule();

    /// <summary>
    ///     Gets the applicant by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    Applicant GetApplicantByApplicationId(Guid applicationId);

    /// <summary>
    ///     Retrieves a list of applicants along with their associated job openings and character references from the database.
    /// </summary>
    /// <returns>
    ///     A List of Applicant objects, each containing their respective Application (including JobOpening) and
    ///     CharacterReferences.
    /// </returns>
    List<Applicant> GetApplicantsWithJobAndReferences(string userAspId);

    /// <summary>
    /// Gets the applicants with exams by job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    List<ApplicantExamViewModel> GetApplicantsWithExamsByJobOpeningId(int jobOpeningId);

    /// <summary>
    /// Gets the confirmed applicants.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    List<ConfirmedApplicantViewModel> GetConfirmedApplicants(int jobOpeningId, string status);
}
using System.Text.Json;
using AutoMapper;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Hangfire;
using Microsoft.IdentityModel.Tokens;

namespace Basecode.Services.Services;

public class TrackService : ITrackService
{
    private readonly IEmailSendingService _emailSendingService;
    private readonly IInterviewService _interviewService;
    private readonly IMapper _mapper;
    private readonly ResumeChecker _resumeChecker;
    private readonly List<string> _statuses;
    private readonly List<string> _interviewStatuses;

    public TrackService(IEmailSendingService emailSendingService, ResumeChecker resumeChecker, IMapper mapper, IInterviewService interviewService)
    {
        _emailSendingService = emailSendingService;
        _interviewService = interviewService;
        _resumeChecker = resumeChecker;
        _mapper = mapper;
        _statuses = new List<string>
        {
            "NA",
            "HR Shortlisted",
            "For HR Screening",
            "For HR Interview",
            "For Technical Exam",
            "For Technical Interview",
            "Technical Shortlisted",
            "Undergoing Background Check",
            "For Final Interview",
            "Undergoing Job Offer",
            "Confirmed",
            "Not Confirmed",
            "Onboarding",
            "Deployed"
        };
        _interviewStatuses = new List<string>
        {
            "For HR Interview",
            "For Technical Interview",
            "For Final Interview",
        };
    }

    /// <summary>
    ///     Checks and sends application status based on resume evaluation.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="applicant">The applicant.</param>
    /// <param name="jobOpening">The job opening.</param>
    /// <returns></returns>
    public async Task<Application?> CheckAndSendApplicationStatus(Application application)
    {
        var result = await _resumeChecker.CheckResume(application.JobOpening.Title,
            application.JobOpening.Qualifications, application.Applicant.CV);

        if (!string.IsNullOrEmpty(result))
        {
            var jsonDocument = JsonDocument.Parse(result);
            var jsonObject = jsonDocument.RootElement;

            var score = jsonObject.GetProperty("Score").GetString();

            if (int.Parse(score.Replace("%", "")) > 60)
                return UpdateApplicationStatus(application, application.JobOpening, "HR Shortlisted", "GUID",
                    result);
        }

        return UpdateApplicationStatus(application, application.JobOpening, "Rejected", "Regret", result);
    }

    /// <summary>
    ///     Updates the track status email.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="mailType">Type of the mail.</param>
    public void UpdateTrackStatusEmail(Application application, User user, string newStatus, string mailType, string oldStatus = "")
    {
        if (application.Applicant.Id >= 0 && user.Id >= -1 && !mailType.IsNullOrEmpty())
        {
            // Map the applicant to a new instance of Applicant without the Application property
            // otherwise HangFire cannot create the background job
            var applicantTemp = _mapper.Map<Applicant>(application.Applicant);
            switch (mailType)
            {
                case "GUID":
                    BackgroundJob.Enqueue(() => _emailSendingService.SendGUIDEmail(application.Id, applicantTemp));
                    break;
                case "Approval":
                    BackgroundJob.Enqueue(() =>
                        _emailSendingService.SendApprovalEmail(user, applicantTemp, application.Id, newStatus));
                    break;
                case "Rejected":
                    BackgroundJob.Enqueue(() => _emailSendingService.SendRejectedEmail(applicantTemp, oldStatus));
                    break;
                case "Regret":
                    RegretNotification(application.Applicant, application.JobOpening.Title);
                    break;
            }
        }
    }

    /// <summary>
    ///     Updates the application status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="mailType">Type of the mail.</param>
    /// <returns></returns>
    public Application UpdateApplicationStatus(Application application, User user, string newStatus,
        string mailType)
    {
        try
        {
            var oldStatus = string.Empty;
            if (newStatus == "Rejected")
                oldStatus = application.Status;

            application.UpdateTime = DateTime.Now;
            application.JobOpening.UpdatedTime = DateTime.Now;
            application.Status = newStatus;

            StatusNotification(application.Applicant, user, newStatus);

            // If newStatus is "For HR Screening" and mailType is "Approval"
            // or if mailType is not "Approval" and mailType is not null/empty
            if ((newStatus == "For HR Screening" && mailType == "Approval") || (mailType != "Approval" && !string.IsNullOrEmpty(mailType)))
            {
                UpdateTrackStatusEmail(application, user, newStatus, mailType, oldStatus);
            }
                
            return application;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    ///     Updates the application status based on the response through email.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="user">The user</param>
    /// <param name="choice">The choice (e.g., approved or rejected).</param>
    /// <param name="newStatus">The new status.</param>
    /// <returns></returns>
    public Application UpdateApplicationStatusByEmailResponse(Application application, User user,
        string choice, string status)
    {
        var newStatus = "";

        if (_interviewStatuses.Contains(status))
        {
            _interviewService.UpdateInterviewResult(application.Id, status, choice);
            if (status == "For Technical Interview")
                return application;
        }

        if (choice.Equals("approved"))
        {
            if (_statuses.Contains(status))
            {
                var statusIndex = _statuses.IndexOf(status);
                newStatus = _statuses[statusIndex + 1];
            }

            return UpdateApplicationStatus(application, user, newStatus, "Approval");
        }

        newStatus = "Rejected";
        //send automated email of regrets
        return UpdateApplicationStatus(application, user, newStatus, "Rejected");
    }

    /// <summary>
    ///     Notifies the applicant and user about the application status.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="user">The user.</param>
    /// <param name="newStatus">The new status.</param>
    public void StatusNotification(Applicant applicant, User user, string newStatus)
    {
        if (applicant.Id >= 0)
        {
            var applicantTemp = _mapper.Map<Applicant>(applicant);
            var userTemp = _mapper.Map<User>(user);
            //Notify HR and Applicant for every status change.
            BackgroundJob.Enqueue(() =>
                _emailSendingService.SendStatusNotification(userTemp, applicantTemp, newStatus));
        }
    }

    /// <summary>
    ///     Notifies the applicant about the regret for not being shortlisted.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="job">The job position.</param>
    public void RegretNotification(Applicant applicant, string job)
    {
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        //Notify Applicant who is not shortlisted upon application
        BackgroundJob.Enqueue(() => _emailSendingService.SendRegretEmail(applicantTemp, job));
    }

    /// <summary>
    ///     Notifies a reference for successful submission of form.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="reference">The reference.</param>
    public void GratitudeNotification(Applicant applicant, BackgroundCheck reference) //TO BE REMOVED
    {
        var referenceTemp = _mapper.Map<BackgroundCheck>(reference);
        //Notify reference for successfully submitting the form
        BackgroundJob.Enqueue(() => _emailSendingService.SendGratitudeEmail(applicant, referenceTemp));
    }

    /// <summary>
    ///     Updates the application status.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <param name="jobOpening">The job opening.</param>
    /// <param name="newStatus">The new status.</param>
    /// <param name="mailType">Type of the mail.</param>
    /// <returns></returns>
    private Application UpdateApplicationStatus(Application application, JobOpening jobOpening,
        string newStatus, string mailType, string result)
    {
        try
        {
            application.UpdateTime = DateTime.Now;
            application.JobOpening.UpdatedTime = DateTime.Now;
            application.Status = newStatus;
            application.Result = result;

            StatusNotification(application.Applicant, jobOpening.Users.First(), newStatus);
            UpdateTrackStatusEmail(application, jobOpening.Users.First(), newStatus, mailType);

            return application;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    /// <summary>
    /// Sends gratitude email to reference and notifies HR for successful completion
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="user"></param>
    /// <param name="applicant"></param>
    /// <returns></returns>
    public async Task SendBackgroundCheckNotification(BackgroundCheck reference, User user, Applicant applicant)
    {
        await _emailSendingService.SendGratitudeEmail(applicant, reference);
        await _emailSendingService.SendBackgroundCheckCompletionToHR(reference, user, applicant);
    }
}
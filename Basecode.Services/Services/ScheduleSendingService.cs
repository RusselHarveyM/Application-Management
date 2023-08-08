using AutoMapper;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Hangfire;

namespace Basecode.Services.Services;

public class ScheduleSendingService : IScheduleSendingService
{
    private readonly IApplicantService _applicantService;
    private readonly IApplicationService _applicationService;
    private readonly IEmailSendingService _emailSendingService;
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ScheduleSendingService(IEmailSendingService emailSendingService, IApplicationService applicationService,
        IJobOpeningService jobOpeningService,
        IApplicantService applicantService, IUserService userService, IMapper mapper)
    {
        _emailSendingService = emailSendingService;
        _applicationService = applicationService;
        _jobOpeningService = jobOpeningService;
        _applicantService = applicantService;
        _userService = userService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Sends the schedule to applicant.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    /// <param name="applicantId">The applicant identifier.</param>
    /// <param name="meetingType">Type of the meeting.</param>
    public void SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, string meetingType,
         int tokenExpiry, int applicantId = -1)
    {
        var applicant = new Applicant();
        if (applicantId == -1) // existing schedule
            applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        else // new schedule
            applicant = _applicantService.GetApplicantById(applicantId);

        var applicantTemp = _mapper.Map<Applicant>(applicant);
        var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
        BackgroundJob.Enqueue(() =>
            _emailSendingService.SendScheduleToApplicant(userScheduleTemp, applicantTemp, tokenExpiry));
    }

    /// <summary>
    ///     Sends the schedules to interviewer.
    /// </summary>
    /// <param name="formData">The form data.</param>
    /// <param name="userId">The user identifier.</param>
    public void SendSchedulesToInterviewer(SchedulerDataViewModel formData, int userId)
    {
        var jobOpeningTitle = _jobOpeningService.GetJobOpeningTitleById(formData.JobOpeningId);
        var user = _userService.GetById(userId);
        var scheduledTimes = "";

        foreach (var schedule in formData.ApplicantSchedules)
        {
            var applicant = _applicantService.GetApplicantById(schedule.ApplicantId);
            scheduledTimes += $"{applicant.Firstname} {applicant.Lastname}'s Schedule: {schedule.Time}<br/>";
        }

        BackgroundJob.Enqueue(() => _emailSendingService.SendSchedulesToInterviewer(user.Fullname, user.Email,
            jobOpeningTitle, formData.Date, scheduledTimes, formData.Type));
    }

    /// <summary>
    ///     Informs the interviewer that a schedule has been rejected.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    public void SendRejectedScheduleNoticeToInterviewer(UserSchedule userSchedule)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var applicantFullName = applicant.Firstname + " " + applicant.Lastname;

        var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
        BackgroundJob.Enqueue(() =>
            _emailSendingService.SendRejectedScheduleNoticeToInterviewer(user.Email, user.Fullname, userScheduleTemp,
                applicantFullName));
    }

    /// <summary>
    ///     Sends the accepted schedule with Teams link to the interviewer.
    /// </summary>
    public void SendAcceptedScheduleToInterviewer(UserSchedule userSchedule, string joinUrl)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var application = _applicationService.GetById(userSchedule.ApplicationId);

        var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
        BackgroundJob.Enqueue(() =>
            _emailSendingService.SendAcceptedScheduleToInterviewer(user.Email, user.Fullname, userScheduleTemp,
                application, joinUrl));
    }

    /// <summary>
    ///     Sends the accepted schedule with Teams link to the applicant.
    /// </summary>
    public void SendAcceptedScheduleToApplicant(UserSchedule userSchedule, string joinUrl)
    {
        var email = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId).Email;
        var application = _applicationService.GetById(userSchedule.ApplicationId);

        var userScheduleTemp = _mapper.Map<UserSchedule>(userSchedule);
        BackgroundJob.Enqueue(() =>
            _emailSendingService.SendAcceptedScheduleToApplicant(email, userScheduleTemp, application, joinUrl));
    }

    /// <summary>
    /// Schedules the sending of the approval email.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    public void ScheduleApprovalEmail(UserSchedule userSchedule, int hoursLeft)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var userTemp = _mapper.Map<User>(user);
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        string newStatus = "For " + userSchedule.Type;
        BackgroundJob.Schedule(() => _emailSendingService.SendApprovalEmail(userTemp, applicantTemp, userSchedule.ApplicationId, newStatus), TimeSpan.FromHours(hoursLeft));
    }

    /// <summary>
    /// Schedules the sending of the exam score reminder email.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    /// <param name="hoursLeft">The hours left until the scheduled time.</param>
    public void ScheduleExamScoreReminderEmail(UserSchedule userSchedule, int hoursLeft)
    {
        var application = _applicationService.GetById(userSchedule.ApplicationId);
        var user = _userService.GetById(userSchedule.UserId);
        BackgroundJob.Schedule(() => _emailSendingService.SendExamScoreReminder(user.Email, user.Fullname, application), TimeSpan.FromHours(hoursLeft));
    }

    public void SendDecisionEmailToInterviewer(UserSchedule userSchedule)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var userTemp = _mapper.Map<User>(user);
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        var newStatus = "For Final Interview";
        var hoursLeft = (int)(userSchedule.Schedule - DateTime.Now).TotalHours;
        BackgroundJob.Schedule(
            () => _emailSendingService.SendDecisionEmail(userTemp, applicantTemp, userSchedule.ApplicationId,
                newStatus), TimeSpan.FromHours(hoursLeft));
    }

    

    public void SendJobOfferEmailToApplicant(UserSchedule userSchedule)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var userTemp = _mapper.Map<User>(user);
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        var newStatus = "Undergoing Job Offer";
        BackgroundJob.Enqueue(() =>
                        _emailSendingService.SendJobOfferEmail(userTemp, applicantTemp, userSchedule.ApplicationId, newStatus));
    }
    public void SendNotifyToDT(int applicantId)
    {
        var user = _userService.GetAll();
        var applicant = _applicantService.GetApplicantById(applicantId);

       

        foreach (var per in user) 
        { 
            if(per.Role == "Deployment Team")
            {
                 _emailSendingService.SendNotifyToDT(applicant, per);
            }
        }
    }
    
    public void ScheduleConfirmationEmailToDT(UserSchedule userSchedule)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var userTemp = _mapper.Map<User>(user);
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        var newStatus = "Confirmed";

        var getAllUser = _userService.GetAll();
        // Calculate the confirmation deadline (2 days before onboarding)
        DateTime onboardingDate = DateTime.UtcNow.AddHours(12);
        DateTime notifyNextDay = DateTime.UtcNow.AddDays(1);

        // Send the confirmation email to DT

        foreach (var per in getAllUser)
        {
            if (per.Role == "Deployment Team")
            {
                // Schedule the confirmation email to be sent on the deadline date
                BackgroundJob.Schedule(() => _emailSendingService.SendConfirmationEmailToDT(per, applicantTemp, userSchedule.ApplicationId, newStatus), onboardingDate);
                BackgroundJob.Schedule(() => _emailSendingService.SendNotifyToDT(applicant, per), notifyNextDay);
            }
        }
    }

    public void SendDeploymentApprovalEmail(UserSchedule userSchedule)
    {
        var user = _userService.GetById(userSchedule.UserId);
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        var userTemp = _mapper.Map<User>(user);
        var applicantTemp = _mapper.Map<Applicant>(applicant);
        var newStatus = "Onboarding";
        var getAllUser = _userService.GetAll();

        foreach (var per in getAllUser)
        {
            if (per.Role == "Human Resources")
            {
                _emailSendingService.SendDeploymentApprovalEmail(per, applicantTemp, userSchedule.ApplicationId, newStatus);
            }
        }
    }

    public void SendCongratulationEmailToApplicant(UserSchedule userSchedule)
    {
        var applicant = _applicantService.GetApplicantByApplicationId(userSchedule.ApplicationId);
        _emailSendingService.SendCongratulationEmailToApplicant(applicant.Email, $"{applicant.Firstname} {applicant.Lastname}");
    }
}
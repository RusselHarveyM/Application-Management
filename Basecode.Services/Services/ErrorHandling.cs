using System.Text.RegularExpressions;
using Basecode.Data.Dto;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;

namespace Basecode.Services.Services;

public class ErrorHandling
{
    public static string SetLog(LogContent logContent)
    {
        return $"ErrorCode: {logContent.ErrorCode}. Message: \"{logContent.Message}\"";
    }

    public static string DefaultException(string message)
    {
        return $"ErrorCode: 500. Message: \"{message}\"";
    }

    public static LogContent CheckJobOpening(JobOpeningViewModel jobOpening)
    {
        var logContent = new LogContent();
        if (string.IsNullOrEmpty(jobOpening.Title) || jobOpening.Title.Length > 50)
        {
            logContent.SetError("400", "Title length is 0 or more than 50 characters.");
            return logContent;
        }

        if (string.IsNullOrEmpty(jobOpening.EmploymentType))
        {
            logContent.SetError("400", "The Employment Type is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(jobOpening.Location))
        {
            logContent.SetError("400", "The Location is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(jobOpening.WorkSetup))
        {
            logContent.SetError("400", "The Work Setup is required but has no value.");
            return logContent;
        }

        if (jobOpening.Qualifications == null || jobOpening.Qualifications.Count == 0)
        {
            logContent.SetError("400", "The Qualifications list is empty.");
            return logContent;
        }

        foreach (var qualification in jobOpening.Qualifications)
            if (string.IsNullOrEmpty(qualification.Description))
            {
                logContent.SetError("400", "Qualification details are empty.");
                return logContent;
            }

        if (jobOpening.Responsibilities == null || jobOpening.Responsibilities.Count == 0)
        {
            logContent.SetError("400", "The Responsibilities list is empty.");
            return logContent;
        }

        foreach (var responsibility in jobOpening.Responsibilities)
            if (string.IsNullOrEmpty(responsibility.Description))
            {
                logContent.SetError("400", "Responsibility details are empty.");
                return logContent;
            }

        return logContent;
    }

    public static LogContent CheckApplicant(ApplicantViewModel applicant)
    {
        var logContent = new LogContent();
        if (string.IsNullOrEmpty(applicant.Firstname))
        {
            logContent.SetError("400", "First Name is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Lastname))
        {
            logContent.SetError("400", "Last Name is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Age.ToString()))
        {
            logContent.SetError("400", "Age is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Birthdate.ToString()))
        {
            logContent.SetError("400", "irthdate is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Gender))
        {
            logContent.SetError("400", "Gender is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Nationality))
        {
            logContent.SetError("400", "Nationality is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Street))
        {
            logContent.SetError("400", "Street is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.City))
        {
            logContent.SetError("400", "City is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Province))
        {
            logContent.SetError("400", "Province is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Zip))
        {
            logContent.SetError("400", "Zip is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Phone))
        {
            logContent.SetError("400", "Phone is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(applicant.Email))
        {
            logContent.SetError("400", "Email is required but has no value.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckCharacterReference(CharacterReferenceViewModel characterReference)
    {
        var logContent = new LogContent();
        if (string.IsNullOrEmpty(characterReference.Name))
        {
            logContent.SetError("400", "Name is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(characterReference.Address))
        {
            logContent.SetError("400", "Address is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(characterReference.Email))
        {
            logContent.SetError("400", "Email is required but has no value.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckUser(UserUpdateViewModel user)
    {
        var logContent = new LogContent();

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        var match = Regex.Match(user.Email, emailPattern);

        if (!match.Success)
        {
            logContent.SetError("400", "The Email Address format is invalid.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckUser(UserViewModel user)
    {
        var logContent = new LogContent();

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        var match = Regex.Match(user.Email, emailPattern);

        if (!match.Success)
        {
            logContent.SetError("400", "The Email Address format is invalid.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckUser(User user)
    {
        var logContent = new LogContent();

        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        var match = Regex.Match(user.Email, emailPattern);

        if (!match.Success)
        {
            logContent.SetError("400", "The Email Address format is invalid.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckApplication(Application existingApplication)
    {
        var logContent = new LogContent();
        if (existingApplication == null) logContent.SetError("404", "Existing application not found.");
        return logContent;
    }

    public static LogContent CheckBackground(BackgroundCheckFormViewModel backgroundCheck)
    {
        var logContent = new LogContent();
        if (backgroundCheck == null)
            logContent.SetError("400", "No data found");
        else if (string.IsNullOrEmpty(backgroundCheck.Email))
            logContent.SetError("400", "Email is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Firstname))
            logContent.SetError("400", "Firstname is required but has no value.d");
        else if (string.IsNullOrEmpty(backgroundCheck.Lastname))
            logContent.SetError("400", "Lastname is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.PhoneNumber))
            logContent.SetError("400", "Phonenumber is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Q1))
            logContent.SetError("400", "Q1 is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Q2))
            logContent.SetError("400", "Q2 is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Q3))
            logContent.SetError("400", "Q3 is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Q4))
            logContent.SetError("400", "Q4 is required but has no value.");
        else if (string.IsNullOrEmpty(backgroundCheck.Relationship))
            logContent.SetError("400", "Q5 is required but has no value.");

        return logContent;
    }

    public static LogContent CheckUserSchedule(UserSchedule schedule)
    {
        var logContent = new LogContent();
        if (schedule == null)
        {
            logContent.SetError("400", "No data found");
            return logContent;
        }

        if (schedule.Schedule == default)
        {
            logContent.SetError("400", "Schedule date is required but has no value.");
            return logContent;
        }

        if (schedule.UserId <= 0)
        {
            logContent.SetError("400", "UserId is invalid.");
            return logContent;
        }

        if (schedule.ApplicationId == Guid.Empty)
        {
            logContent.SetError("400", "ApplicationId is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(schedule.Type))
        {
            logContent.SetError("400", "Schedule Type is required but has no value.");
            return logContent;
        }

        if (string.IsNullOrEmpty(schedule.Status))
        {
            logContent.SetError("400", "Schedule Status is required but has no value.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckUserScheduleStatus(UserSchedule schedule)
    {
        var logContent = new LogContent();
        if (schedule == null)
        {
            logContent.SetError("404", "Schedule is not found.");
            return logContent;
        }

        if (schedule.Status == "accepted")
        {
            logContent.SetError("401", "Schedule has already been accepted.");
            return logContent;
        }

        if (schedule.Status == "rejected")
        {
            logContent.SetError("401", "Schedule has already been rejected.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckCalendarEvent(CalendarEvent calendarEvent)
    {
        var logContent = new LogContent();
        if (calendarEvent == null)
        {
            logContent.SetError("400", "Calendar event is null.");
            return logContent;
        }

        if (string.IsNullOrEmpty(calendarEvent.Subject))
        {
            logContent.SetError("400", "Calendar event's Subject is null or empty.");
            return logContent;
        }

        if (string.IsNullOrEmpty(calendarEvent.Body.Content))
        {
            logContent.SetError("400", "Calendar event's Body is null or empty.");
            return logContent;
        }

        if (calendarEvent.Start.DateTime == default)
        {
            logContent.SetError("400", "Calendar event's Start DateTime is not set.");
            return logContent;
        }

        if (calendarEvent.End.DateTime == default)
        {
            logContent.SetError("400", "Calendar event's End DateTime is not set.");
            return logContent;
        }

        if (calendarEvent.OnlineMeetingProvider != "TeamsForBusiness")
        {
            logContent.SetError("400", "Calendar event's Online Meeting Provider is invalid.");
            return logContent;
        }

        if (calendarEvent.IsOnlineMeeting == false)
        {
            logContent.SetError("400", "Calendar event is not an online meeting.");
            return logContent;
        }

        return logContent;
    }

    public static (LogContent, Dictionary<string, string>) CheckSchedulerData(SchedulerDataViewModel formData)
    {
        var validationErrors = new Dictionary<string, string>();
        var logContent = new LogContent();
        if (formData.JobOpeningId <= 0)
        {
            logContent.SetError("400", "JobOpeningId is invalid.");
            validationErrors.Add("JobOpeningId", "Please select a job opening.");
            return (logContent, validationErrors);
        }

        if (string.IsNullOrEmpty(formData.Type))
        {
            logContent.SetError("400", "Type is required but has no value.");
            validationErrors.Add("Type", "Please select a type.");
            return (logContent, validationErrors);
        }

        if (formData.Date == default)
        {
            logContent.SetError("400", "Date is required but has not been set.");
            validationErrors.Add("Date", "Please select a date.");
            return (logContent, validationErrors);
        }

        if (formData.ApplicantSchedules == null || formData.ApplicantSchedules.Count == 0)
        {
            logContent.SetError("400", "The ApplicantSchedules list is empty.");
            validationErrors.Add("Applicant", "Please select an applicant.");
            return (logContent, validationErrors);
        }

        foreach (var applicant in formData.ApplicantSchedules)
            if (string.IsNullOrEmpty(applicant.Time) || applicant.Time == " ")
            {
                logContent.SetError("400", "Time is required but has not been set.");
                validationErrors.Add("Time", "Please set a time.");
                return (logContent, validationErrors);
            }

        return (logContent, validationErrors);
    }

    public static LogContent CheckCurrentHireStatus(CurrentHire hire)
    {
        var logContent = new LogContent();
        if (hire == null)
        {
            logContent.SetError("404", "Current hire is not found.");
            return logContent;
        }

        if (hire.Status == "accepted")
        {
            logContent.SetError("401", "Offer has already been accepted.");
            return logContent;
        }

        if (hire.Status == "rejected")
        {
            logContent.SetError("401", "Offer has already been rejected.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckCurrentHire(CurrentHire hire)
    {
        var logContent = new LogContent();
        if (hire == null)
        {
            logContent.SetError("400", "No data found");
            return logContent;
        }

        if (hire.UserId <= 0)
        {
            logContent.SetError("400", "UserId is invalid.");
            return logContent;
        }

        if (hire.ApplicationId == Guid.Empty)
        {
            logContent.SetError("400", "ApplicationId is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(hire.Status))
        {
            logContent.SetError("400", "Offer Status is required but has no value.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckExamination(Examination examination)
    {
        var logContent = new LogContent();
        if (examination == null)
        {
            logContent.SetError("404", "Schedule is not found.");
            return logContent;
        }

        if (Equals(examination.ApplicationId, Guid.Empty))
        {
            logContent.SetError("400", "ApplicationId is invalid.");
            return logContent;
        }

        if (examination.UserId == 0 || examination.UserId == null)
        {
            logContent.SetError("400", "UserId is invalid.");
            return logContent;
        }

        if (examination.Date == default || examination.Date == null)
        {
            logContent.SetError("400", "Date is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(examination.TeamsLink))
        {
            logContent.SetError("400", "TeamsLink is invalid.");
            return logContent;
        }

        if (examination.Score == 0 || examination.Score == null)
        {
            logContent.SetError("400", "Score is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(examination.Result))
        {
            logContent.SetError("400", "Result is invalid.");
            return logContent;
        }

        return logContent;
    }

    public static LogContent CheckInterview(Interview interview)
    {
        var logContent = new LogContent();
        if (interview == null)
        {
            logContent.SetError("404", "Schedule is not found.");
            return logContent;
        }

        if (Equals(interview.ApplicationId, Guid.Empty))
        {
            logContent.SetError("400", "ApplicationId is invalid.");
            return logContent;
        }

        if (interview.UserId == 0 || interview.UserId == null)
        {
            logContent.SetError("400", "UserId is invalid.");
            return logContent;
        }

        if (interview.Date == default || interview.Date == null)
        {
            logContent.SetError("400", "Date is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(interview.TeamsLink))
        {
            logContent.SetError("400", "TeamsLink is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(interview.Type))
        {
            logContent.SetError("400", "Type is invalid.");
            return logContent;
        }

        if (string.IsNullOrEmpty(interview.Result))
        {
            logContent.SetError("400", "Result is invalid.");
            return logContent;
        }

        return logContent;
    }

    public class LogContent
    {
        public string ErrorCode { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Result { get; set; }

        public void SetError(string errorCode, string errorMessage)
        {
            Result = true;
            ErrorCode = errorCode;
            Message = errorMessage;
        }
    }
}
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace Basecode.Services.Services
{
    public class ErrorHandling
    {
        public class LogContent
        {
            public string ErrorCode { get; set; } = string.Empty;
            public DateTime Time { get; set; }
            public string Message { get; set; } = string.Empty;
            public bool Result { get; set; } = false;

            public void SetError(string errorCode, string errorMessage)
            {
                Result = true;
                ErrorCode = errorCode;
                Message = errorMessage;
            }
        }

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
            LogContent logContent = new LogContent();
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
            else
            {
                foreach (var qualification in jobOpening.Qualifications)
                {
                    if (string.IsNullOrEmpty(qualification.Description))
                    {
                        logContent.SetError("400", "Qualification details are empty.");
                        return logContent;
                    }
                }
            }

            if (jobOpening.Responsibilities == null || jobOpening.Responsibilities.Count == 0)
            {
                logContent.SetError("400", "The Responsibilities list is empty.");
                return logContent;
            }
            else
            {
                foreach (var responsibility in jobOpening.Responsibilities)
                {
                    if (string.IsNullOrEmpty(responsibility.Description))
                    {
                        logContent.SetError("400", "Responsibility details are empty.");
                        return logContent;
                    }
                }
            }

            return logContent;
        }

        public static LogContent CheckApplicant(ApplicantViewModel applicant)
        {
            LogContent logContent = new LogContent();
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
            LogContent logContent = new LogContent();
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

        public static LogContent CheckUser(User user)
        {
            LogContent logContent = new LogContent();

            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Match match = Regex.Match(user.Email, emailPattern);

            if (!match.Success)
            {
                logContent.SetError("400", "The Email Address format is invalid.");
                return logContent;
            }

            return logContent;
        }

        public static LogContent CheckApplication(Application existingApplication)
        {
            LogContent logContent = new LogContent();
            if (existingApplication == null)
            {
                logContent.SetError("404", "Existing application not found.");
            }
            return logContent;
        }

        public static LogContent CheckBackground(BackgroundCheckFormViewModel backgroundCheck)
        {
            LogContent logContent = new LogContent();
            if(backgroundCheck == null)
            {
                logContent.SetError("400", "No data found");
            }else if (string.IsNullOrEmpty(backgroundCheck.Email))
            {
                logContent.SetError("400", "Email is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Firstname))
            {
                logContent.SetError("400", "Firstname is required but has no value.d");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Lastname))
            {
                logContent.SetError("400", "Lastname is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.PhoneNumber))
            {
                logContent.SetError("400", "Phonenumber is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Q1))
            {
                logContent.SetError("400", "Q1 is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Q2))
            {
                logContent.SetError("400", "Q2 is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Q3))
            {
                logContent.SetError("400", "Q3 is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Q4))
            {
                logContent.SetError("400", "Q4 is required but has no value.");
            }
            else if (string.IsNullOrEmpty(backgroundCheck.Relationship))
            {
                logContent.SetError("400", "Q5 is required but has no value.");
            }

            return logContent;
        }
    }
}

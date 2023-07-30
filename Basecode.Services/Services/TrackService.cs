using AutoMapper;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Hangfire;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class TrackService : ITrackService
    {
        private readonly IEmailSendingService _emailSendingService;
        private readonly ResumeChecker _resumeChecker;
        private readonly List<string> _statuses;
        private readonly IMapper _mapper;

        public TrackService(IEmailSendingService emailSendingService, ResumeChecker resumeChecker, IMapper mapper)
        {
            _emailSendingService = emailSendingService;
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
                "For Final Interview"
            };
        }

        /// <summary>
        /// Checks and sends application status based on resume evaluation.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="jobOpening">The job opening.</param>
        /// <returns></returns>
        public async Task<Application?> CheckAndSendApplicationStatus(Application application)
        {
            var result = await _resumeChecker.CheckResume(application.JobOpening.Title, application.JobOpening.Qualifications, application.Applicant.CV);
            //var result = "test";

            if (!string.IsNullOrEmpty(result))
            {
                JsonDocument jsonDocument = JsonDocument.Parse(result);
                var jsonObject = jsonDocument.RootElement;


                // Accessing individual properties
                string jobPosition = jsonObject.GetProperty("JobPosition").GetString();
                string score = jsonObject.GetProperty("Score").GetString();
                string explanation = jsonObject.GetProperty("Explanation").GetString();

                //int convertedScore = 70;
                if (int.Parse(score.Replace("%", "")) > 60)
                //if (convertedScore > 60)
                {
                    return UpdateApplicationStatus(application, application.JobOpening, "HR Shortlisted", "GUID");
                }
                else
                {
                    RegretNotification(application.Applicant, application.JobOpening.Title);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates the track status email.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        public void UpdateTrackStatusEmail(Application application, User user, string newStatus, string mailType)
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
                        BackgroundJob.Enqueue(() => _emailSendingService.SendApprovalEmail(user, applicantTemp, application.Id, newStatus));
                        break;
                    case "Rejected":
                        BackgroundJob.Enqueue(() => _emailSendingService.SendRejectedEmail(applicantTemp, newStatus));
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the application status.
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
                application.UpdateTime = DateTime.Now;
                application.Status = newStatus;

                StatusNotification(application.Applicant, user, newStatus);
                UpdateTrackStatusEmail(application, user, newStatus, mailType);
                return application;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Updates the application status.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="jobOpening">The job opening.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        /// <returns></returns>
        private Application UpdateApplicationStatus(Application application, JobOpening jobOpening,
            string newStatus, string mailType)
        {
            try
            {
                application.UpdateTime = DateTime.Now;
                application.Status = newStatus;

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
        /// Updates the application status based on the response through email.
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

            if (choice.Equals("approved"))
            {
                if (_statuses.Contains(status))
                {
                    var statusIndex = _statuses.IndexOf(status);
                    newStatus = _statuses[statusIndex + 1];
                }
                return UpdateApplicationStatus(application, user, newStatus, "Approval");
            }
            else
            {
                //send automated email of regrets
                return UpdateApplicationStatus(application, user, newStatus, "Rejected");
            }
        }

        /// <summary>
        /// Notifies the applicant and user about the application status.
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
                BackgroundJob.Enqueue(() => _emailSendingService.SendStatusNotification(userTemp, applicantTemp, newStatus));
            }
        }

        /// <summary>
        /// Notifies the applicant about the regret for not being shortlisted.
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
        /// Notifies a reference for successful submission of form.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="reference">The reference.</param>
        public void GratitudeNotification(Applicant applicant, BackgroundCheck reference)
        {
            var referenceTemp = _mapper.Map<BackgroundCheck>(reference);
            //Notify reference for successfully submitting the form
            BackgroundJob.Enqueue(() => _emailSendingService.SendGratitudeEmail(applicant, referenceTemp));
        }
    }
}
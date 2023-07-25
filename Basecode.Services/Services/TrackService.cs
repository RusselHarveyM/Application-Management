using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
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

        public TrackService(IEmailSendingService emailSendingService, ResumeChecker resumeChecker)
        {
            _emailSendingService = emailSendingService;
            _resumeChecker = resumeChecker;
        }

        /// <summary>
        /// Checks and sends application status based on resume evaluation.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="jobOpening">The job opening.</param>
        /// <returns></returns>
        public async Task<Application> CheckAndSendApplicationStatus(Application application, Applicant applicant, JobOpening jobOpening)
        {
            var result = await _resumeChecker.CheckResume(jobOpening.Title, applicant.CV);

            JsonDocument jsonDocument = JsonDocument.Parse(result);
            var jsonObject = jsonDocument.RootElement;

            // Accessing individual properties
            string jobPosition = jsonObject.GetProperty("JobPosition").GetString();
            string score = jsonObject.GetProperty("Score").GetString();
            string explanation = jsonObject.GetProperty("Explanation").GetString();

            if (int.Parse(score.Replace("%", "")) > 60)
            {

                return await UpdateApplicationStatus(application, jobOpening, "HR Shortlisted", "GUID");
            }
            else
            {
                await RegretNotification(applicant, jobOpening.Title);
                return null;
            }
        }

        /// <summary>
        /// Updates the track status email.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        public async Task UpdateTrackStatusEmail(Application application, User user, string newStatus, string mailType)
        {
            if (application.Applicant.Id >= 0 && user.Id >= -1 && !mailType.IsNullOrEmpty())
            {
                switch (mailType)
                {
                    case "GUID":
                        await _emailSendingService.SendGUIDEmail(application);
                        break;
                    case "Approval":
                        await _emailSendingService.SendApprovalEmail(user, application.Applicant, application.Id, newStatus);
                        break;
                    case "Rejected":
                        await _emailSendingService.SendRejectedEmail(application.Applicant, newStatus);
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
        public async Task<Application> UpdateApplicationStatus(Application application, User user, string newStatus, string mailType)
        {
            try
            {
                application.UpdateTime = DateTime.Now;
                application.Status = newStatus;

                await StatusNotification(application.Applicant, user, newStatus);
                await UpdateTrackStatusEmail(application, user, newStatus, mailType);
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
        private async Task<Application> UpdateApplicationStatus(Application application, JobOpening jobOpening, string newStatus, string mailType)
        {
            try
            {
                application.UpdateTime = DateTime.Now;
                application.Status = newStatus;

                await StatusNotification(application.Applicant, jobOpening.Users.First(), newStatus);
                await UpdateTrackStatusEmail(application, jobOpening.Users.First(), newStatus, mailType);
                return application;
            }
            catch(Exception e)
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
        public async Task<Application> UpdateApplicationStatusByEmailResponse(Application application, User user, string choice, string newStatus)
        {
            if (choice.Equals("approved"))
            {
                return await UpdateApplicationStatus(application, user, newStatus, "Approval");
            }
            else
            {
                //send automated email of regrets
                return await UpdateApplicationStatus(application, user, newStatus, "Rejected");
            }
        }

        /// <summary>
        /// Notifies the applicant and user about the application status.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        public async Task StatusNotification(Applicant applicant, User user, string newStatus)
        {
            if (applicant.Id >= 0)
            {
                //Notify HR and Applicant for every status change.
                await _emailSendingService.SendStatusNotification(user, applicant, newStatus);
            }
        }

        /// <summary>
        /// Notifies the applicant about the regret for not being shortlisted.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="job">The job position.</param>
        public async Task RegretNotification(Applicant applicant, string job)
        {
            //Notify Applicant who is not shortlisted upon application
            await _emailSendingService.SendRegretEmail(applicant, job);
        }

        /// <summary>
        /// Notifies a reference for successful submission of form.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="reference">The reference.</param>
        public async Task GratitudeNotification(Applicant applicant, BackgroundCheck reference)
        {
            //Notify reference for successfully submitting the form
            await _emailSendingService.SendGratitudeEmail(applicant, reference);
        }
    }
}

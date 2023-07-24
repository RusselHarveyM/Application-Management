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

        public async Task StatusNotification(Applicant applicant, User user, string newStatus)
        {
            if (applicant.Id >= 0)
            {
                //Notify HR and Applicant for every status change.
                await _emailSendingService.SendStatusNotification(user, applicant, newStatus);
            }
        }

        public async Task RegretNotification(Applicant applicant, string job)
        {
            //Notify Applicant who is not shortlisted upon application
            await _emailSendingService.SendRegretEmail(applicant, job);
        }
    }
}

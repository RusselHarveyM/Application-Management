using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Basecode.Data.Models;

namespace Basecode.Services.Services
{
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Sends an email using the specified SMTP client with the provided recipient, subject, and body.
        /// </summary>
        /// <param name="recipient">The email address of the recipient.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SendEmail(string recipient, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp-mail.outlook.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("hrautomatesystem@outlook.com", "alliance2023");

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("hrautomatesystem@outlook.com");
                    mailMessage.To.Add(recipient);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            await Task.CompletedTask;
        }

        public async Task SendNotifyEmail(Applicant applicant, string newStatus)
        {
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Application Status Tracker")
                .Replace("{{BODY}}", $"Dear {applicant.Firstname},<br>" +
                                     $"<br> We are reaching out to provide you with your application ID for tracking purposes. Please find the details below:<br>" +
                                     $"<br> <b>Application ID: [{applicant.Application.Id}]</b> <br>" +
                                     $"<br> This unique ID will allow you to track the progress of your application. To do so, please follow these steps: <br>" +
                                     $"<br> 1. Visit Alliance Software Inc. website at [website URL]." +
                                     $"<br> 2. Navigate to the application tracker section." +
                                     $"<br> 3. Enter your Application ID: {applicant.Application.Id}" +
                                     $"<br> 4. Click the \"Track\" button. <br>" +
                                     $"<br> The tracking system will provide real-time updates on the status of your application, from submission to evaluation. We appreciate your patience throughout the process. <br>" +
                                     $"<br> If you have any questions or encounter any issues, please reach out to our support team at [contact email/phone number]. <br>" +
                                     $"<br> Thank you for choosing Alliance Software Inc. We wish you the best of luck with your application!");

            await this.SendEmail(applicant.Email, "Alliance Software Inc. Application Status Tracker", body);
        }

        public async Task SendStatusNotification(User user, Applicant applicant, string newStatus)
        {
            //Notify Applicant
            var templatePath = Path.Combine("wwwroot", "template", "mailtemplate.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Applicant Status")
                .Replace("{{BODY}}", $"Dear {applicant.Firstname},<br> Application [{applicant.Id}] has changed its status. <br> Current Status: {newStatus}");

            await this.SendEmail(applicant.Email, "Alliance Software Inc. Applicant Status Update", body);

            //Notify HR
            await this.SendEmail(user.Email, "Applicant Status Update for HR",
               $"Applicant {applicant.Firstname} (ID: {applicant.Id}) has changed status to {newStatus}.");
        }

        public async Task SendApprovalEmail(User user, Applicant applicant, Guid appId, string newStatus)
        {
            var templatePath = Path.Combine("wwwroot", "template", "ApprovalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Approval Email")
                .Replace("{{BODY}}", $"Dear {user.Fullname},<br> Application [{applicant.Id}] has just finished their interview, please provide your feedback to" +
                    $" proceed to the next phase. Thank you.")
                .Replace("{{APPLICATION_ID}}", $"{ appId }")
                .Replace("{{USER_ID}}", $"{user.Id}")
                .Replace("{{STATUS}}", $"{newStatus}");

            await this.SendEmail(user.Email, "Alliance Software Inc. Applicant Status Update", body);
        }

        public async Task SendRejectedEmail(Applicant applicant, string newStatus)
        {
            var redirectLink = "https://localhost:50991/Home";
            await this.SendEmail(applicant.Email, "Applicant Status Update for Applicant",
                $"<b>Dear {applicant.Firstname},</b> <br><br> Thank you for submitting your application. We have received it successfully and appreciate your interest in joining our team. <br><br> <b>Status:</b> {newStatus}. " +
                $"<br><br> <em>This is an automated messsage. Do not reply</em> <br><br> <a href=\"{redirectLink}\" " +
                $"style=\"background-color: #FF0000; border: none; color: white; padding: 10px 24px; text-align: center; text-decoration: underline; " +
                $"display: inline-block; font-size: 14px; margin: 4px 2px; cursor: pointer;\">Visit Alliance</a>");
        }
    }
}

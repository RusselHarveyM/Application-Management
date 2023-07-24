using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class EmailSendingService : IEmailSendingService
    {
        private readonly IEmailService _emailService;

        public EmailSendingService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task SendInterviewNotification(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {
            //Notify Interviewer for their Task
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Assign Interviewer")
                .Replace("{{BODY}}", $"Dear {intervierwerFullName},<br>" +
                                     $"<br> The Job Position you being assigned to interview is the {jobPosition}. Please see details below.<br>" +
                                     $"<br> Your Credentials Email: {interviewerEmail} Password: {interviewerPassword}<br>");

            await _emailService.SendEmail(interviewerEmail, "Alliance Software Inc. Hello Deployment Team", body);
        }


        public async Task SendInterviewNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {


            //Notify Interviewer for their Task
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Assign Interviewer")
                .Replace("{{BODY}}", $"Dear {intervierwerFullName},<br>" +
                                     $"<br> The Job Position you being assigned to interview is the {jobPosition}. Please see details below.<br>" +
                                     $"<br> Your Credentials Email: {interviewerEmail} Password: {interviewerPassword}<br>");

            await _emailService.SendEmail(interviewerEmail, "Alliance Software Inc. Hello Deployment Team", body);
        }

        public async Task SendHrNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {


            //Notify Interviewer for their Task
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Assign Interviewer")
                .Replace("{{BODY}}", $"Dear {intervierwerFullName},<br>" +
                                     $"<br> The Job Position you being assigned to interview is the {jobPosition}. Please see details below.<br>" +
                                     $"<br> Your Credentials Email: {interviewerEmail} Password: {interviewerPassword}<br>");

            await _emailService.SendEmail(interviewerEmail, "Alliance Software Inc. Hello Human Resource", body);
        }

        public async Task SendTechnicalNotif2weeks(string interviewerEmail, string intervierwerFullName, string interviewerUsername,
                               string interviewerPassword, string jobPosition)
        {


            //Notify Interviewer for their Task
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Assign Interviewer")
                .Replace("{{BODY}}", $"Dear {intervierwerFullName},<br>" +
                                     $"<br> The Job Position you being assigned to interview is the {jobPosition}. Please see details below.<br>" +
                                     $"<br> Your Credentials Email: {interviewerEmail} Password: {interviewerPassword}<br>");

            await _emailService.SendEmail(interviewerEmail, "Alliance Software Inc. Hello Technical", body);
        }

        /// <summary>
        /// Sends an email to an applicant containing their Unique Tracking ID.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SendGUIDEmail(Application application)
        {
            //Notify Applicant for their Unique Tracking ID
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Application Status Tracker")
                .Replace("{{BODY}}", $"Dear {application.Applicant.Firstname},<br>" +
                                     $"<br> We are reaching out to provide you with your application ID for tracking purposes. Please find the details below:<br>" +
                                     $"<br> <b>Application ID: [{application.Id}]</b> <br>" +
                                     $"<br> This unique ID will allow you to track the progress of your application. To do so, please follow these steps: <br>" +
                                     $"<br> 1. Visit Alliance Software Inc. website at [website URL]." +
                                     $"<br> 2. Navigate to the application tracker section." +
                                     $"<br> 3. Enter your Application ID: {application.Id}" +
                                     $"<br> 4. Click the \"Track\" button. <br>" +
                                     $"<br> The tracking system will provide real-time updates on the status of your application, from submission to evaluation. We appreciate your patience throughout the process. <br>" +
                                     $"<br> If you have any questions or encounter any issues, please reach out to our support team at [contact email/phone number]. <br>" +
                                     $"<br> Thank you for choosing Alliance Software Inc. We wish you the best of luck with your application!");

            await _emailService.SendEmail(application.Applicant.Email, "Alliance Software Inc. Application Status Tracker", body);
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

            await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Applicant Status Update", body);

            //Notify HR
            await _emailService.SendEmail(user.Email, "Applicant Status Update for HR",
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
                .Replace("{{BODY}}", $"Dear {user.Fullname},<br> Application [{applicant.Id}] is ready for HR Screening, please provide your feedback to" +
                    $" proceed to the next phase. Thank you.")
                .Replace("{{APPLICATION_ID}}", $"{appId}")
                .Replace("{{USER_ID}}", $"{user.Id}")
                .Replace("{{STATUS}}", $"{newStatus}");

            await _emailService.SendEmail(user.Email, "Alliance Software Inc. Applicant Status Update", body);
        }

        public async Task SendRejectedEmail(Applicant applicant, string newStatus)
        {
            var redirectLink = "https://localhost:50991/Home";
            await _emailService.SendEmail(applicant.Email, "Applicant Status Update for Applicant",
                $"<b>Dear {applicant.Firstname},</b> <br><br> Thank you for submitting your application. We have received it successfully and appreciate your interest in joining our team. <br><br> <b>Status:</b> {newStatus}. " +
                $"<br><br> <em>This is an automated messsage. Do not reply</em> <br><br> <a href=\"{redirectLink}\" " +
                $"style=\"background-color: #FF0000; border: none; color: white; padding: 10px 24px; text-align: center; text-decoration: underline; " +
                $"display: inline-block; font-size: 14px; margin: 4px 2px; cursor: pointer;\">Visit Alliance</a>");
        }

        public async Task SendRegretEmail(Applicant applicant, string job)
        {
            //For applicants who were not shortlisted upon application
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Application Status")
                .Replace("{{BODY}}", $"Dear {applicant.Firstname},<br>" +
                                     $"<br> I hope this email finds you well. We appreciate your interest in the {job} position here at Alliance Software Inc. Thank you for taking the time to apply and share your qualifications with us. <br>" +
                                     $"<br> After carefully reviewing your application, we regret to inform you that your profile does not align closely with the specific qualifications and requirements we are seeking for this role. Our selection process was highly competitive, and we received numerous applications from candidates who possess the exact skills and experience needed for the position. <br>" +
                                     $"<br> While your experience and achievements are impressive, we have decided to move forward with other candidates whose qualifications more closely match our current needs. <br>" +
                                     $"<br> Please know that this decision does not reflect on your capabilities or potential. We recognize the effort you put into your application, and we sincerely appreciate your interest in joining our team. <br>" +
                                     $"<br> As a company, we strive to ensure that every applicant receives a fair evaluation. We encourage you to continue exploring other opportunities within our organization or elsewhere that may be a better fit for your skills and aspirations. <br>" +
                                     $"<br> Once again, thank you for your interest in Alliance Software Inc. We wish you all the best in your job search and future endeavors." +
                                     $"<br> If you have any questions or would like feedback on your application, please feel free to reach out. We're more than willing to provide insights that may be helpful in your career journey. <br>" +
                                     $"<br> Best regards, <br>");

            await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Application Status Update", body);
        }
    }
}

using Basecode.Data.Models;
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

        /// <summary>
        /// Sends the interview notification.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
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


        /// <summary>
        /// Sends the interview notification every 2 weeks.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
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

        /// <summary>
        /// Sends the hr notifications every 2 weeks.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
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

        /// <summary>
        /// Sends the technical notifications every 2 weeks.
        /// </summary>
        /// <param name="interviewerEmail">The interviewer email.</param>
        /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
        /// <param name="interviewerUsername">The interviewer username.</param>
        /// <param name="interviewerPassword">The interviewer password.</param>
        /// <param name="jobPosition">The job position.</param>
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
        /// <param name="application">The application.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
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

        /// <summary>
        /// Sends the status notification.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
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

        /// <summary>
        /// Sends the approval email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="applicant">The applicant.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="newStatus">The new status.</param>
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

        /// <summary>
        /// Sends the rejected email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        public async Task SendRejectedEmail(Applicant applicant, string newStatus)
        {
            var redirectLink = "https://localhost:50991/Home";
            await _emailService.SendEmail(applicant.Email, "Applicant Status Update for Applicant",
                $"<b>Dear {applicant.Firstname},</b> <br><br> Thank you for submitting your application. We have received it successfully and appreciate your interest in joining our team. <br><br> <b>Status:</b> {newStatus}. " +
                $"<br><br> <em>This is an automated messsage. Do not reply</em> <br><br> <a href=\"{redirectLink}\" " +
                $"style=\"background-color: #FF0000; border: none; color: white; padding: 10px 24px; text-align: center; text-decoration: underline; " +
                $"display: inline-block; font-size: 14px; margin: 4px 2px; cursor: pointer;\">Visit Alliance</a>");
        }

        /// <summary>
        /// Sends the regret email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="job">The job.</param>
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

        /// <summary>
        /// Sends the schedule to applicant.
        /// </summary>
        public async Task SendScheduleToApplicant(UserSchedule userSchedule, int userScheduleId, Applicant applicant, string meetingType)
        {
            var ACCEPT_URL = "https://localhost:53813/Scheduler/AcceptSchedule";
            var REJECT_URL = "https://localhost:53813/Scheduler/RejectSchedule";
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", $"{meetingType} Schedule")
                .Replace("{{BODY}}", $"Dear {applicant.Firstname},<br>" +
                                     $"<br> Your {meetingType} has been scheduled for {userSchedule.Schedule}.<br/>" +
                                     $"<br> Please click the button to accept or reject the schedule:<br/>" +
                                     $"<br> <a href=\"{ACCEPT_URL}?userScheduleId={userScheduleId}\">Accept</a> " +
                                     $"<a href=\"{REJECT_URL}?userScheduleId={userScheduleId}\">Reject</a>");

            await _emailService.SendEmail(applicant.Email, $"Alliance Software Inc. Application {meetingType} Schedule", body);
        }

        /// <summary>
        /// Sends the schedules to the interviewer.
        /// </summary>
        public async Task SendSchedulesToInterviewer(string fullname, string userEmail, string jobOpeningTitle, DateOnly date, string scheduledTimes, string meetingType)
        {
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", $"{meetingType} Schedule")
                .Replace("{{BODY}}", $"Dear {fullname},<br>" +
                                     $"<br> Here is an overview of the {meetingType} schedules you have set for the position of {jobOpeningTitle} on {date}:<br>" +
                                     $"<br> {scheduledTimes}");

            await _emailService.SendEmail(userEmail, $"Alliance Software Inc. {meetingType} Schedules", body);
        }

        public async Task SendRequestReference(CharacterReference reference, Applicant applicant)
        {
            var url = "https://localhost:49940/BackgroundCheck/Index";
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Character Reference")
                .Replace("{{BODY}}", $"Dear {reference.Name},<br>" +
                                     $"<br> I hope this email finds you well. We appreciate your willingness to provide a character reference for the applicant, {applicant.Firstname} {applicant.Lastname}<br/>" +
                                     $"<br> To proceed with the application process, we kindly request you to complete the Character Reference Form by following the link below:<br/>" +
                                     $"<br> <a href=\"{url}\">Character Reference Form</a> ");

            await _emailService.SendEmail(reference.Email, $"Character Reference Form Request", body);
        }

        /// <summary>
        /// Sends the gratitude email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="reference">The reference.</param>
        public async Task SendGratitudeEmail(Applicant applicant, CharacterReference reference)
        {
            var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
            var templateContent = File.ReadAllText(templatePath);
            var body = templateContent
                .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                .Replace("{{HEADLINE}}", "Character Reference Form Completion")
                .Replace("{{BODY}}", $"Dear {reference.Name},<br>" +
                                     $"<br> I hope this email finds you well. I wanted to extend my heartfelt appreciation for taking the time to complete the Character Reference Form on behalf of Mr/Ms. {applicant.Firstname} {applicant.Lastname}. <br>" +
                                     $"<br> Your thoughtful and positive feedback has been received and will play a crucial role in the evaluation process for Mr/Ms. {applicant.Lastname}'s application. Character references are incredibly valuable in providing insights into an individual's personal attributes and qualities, and your input will help us make an informed decision. <br>" +
                                     $"<br> We understand that providing a character reference requires time and effort, and we are truly grateful for your support in this matter. Your willingness to vouch for Mr/Ms. {applicant.Lastname}'s character speaks volumes about your relationship with them, and it is a testament to their strengths and character. <br>" +
                                     $"<br> If you have any questions or need any further information about the application process, please do not hesitate to reach out to us. Your assistance in this matter is invaluable, and we are here to assist you in any way we can. <br>" +
                                     $"<br> Once again, thank you for being a part of Mr/Ms. {applicant.Lastname}'s journey through this process. We truly appreciate your contribution to our decision-making process." +
                                     $"<br> Best regards,");

            await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Background Check", body);
        }
    }
}

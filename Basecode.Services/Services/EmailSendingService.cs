using System.Text;
using System.Web;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Basecode.Services.Services;

public class EmailSendingService : IEmailSendingService
{
    private readonly IEmailService _emailService;
    private readonly TokenHelper _tokenHelper;
    private readonly IUserRepository _userRepository;

    public EmailSendingService(IEmailService emailService, IUserRepository userRepository, IConfiguration config)
    {
        _emailService = emailService;
        _userRepository = userRepository;
        _tokenHelper = new TokenHelper(config["TokenHelper:SecretKey"]);
    }

    /// <summary>
    ///     Sends the interview notification.
    /// </summary>
    /// <param name="interviewerEmail">The interviewer email.</param>
    /// <param name="intervierwerFullName">Full name of the intervierwer.</param>
    /// <param name="interviewerUsername">The interviewer username.</param>
    /// <param name="interviewerPassword">The interviewer password.</param>
    /// <param name="jobPosition">The job position.</param>
    /// <param name="role">The role</param>
    public async Task SendInterviewNotification(string interviewerEmail, string intervierwerFullName,
        string interviewerUsername,
        string interviewerPassword, string jobPosition, string role)
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

        await _emailService.SendEmail(interviewerEmail, role, body);
    }
    
    public async Task SendReferenceAnswers(User user, Applicant applicant, Guid appId, string newStatus, List<byte[]> pdfs)
    {
        var tokenClaims = new Dictionary<string, string>
        {
            { "action", "ChangeStatus" },
            { "userId", user.Id.ToString() },
            { "appId", appId.ToString() },
            { "newStatus", newStatus },
            { "choice", "approved" }
        };
        var approveToken = _tokenHelper.GenerateToken(tokenClaims);

        tokenClaims["choice"] = "rejected";
        var rejectToken = _tokenHelper.GenerateToken(tokenClaims);
        
        //Notify Interviewer for their Task
        var templatePath = Path.Combine("wwwroot", "template", "ApprovalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "An Applicant's Referee Has Answered.")
            .Replace("{{BODY}}", $"Dear {user.Fullname},<br>" +
                                 $"<br> These are all the answers of applicant [{appId}]'s referees who answered the form within the time frame.<br>")
            .Replace("{{REJECT_TOKEN}}", $"{rejectToken}")
            .Replace("{{NEGATIVE_FEEDBACK}}", "Reject")
            .Replace("{{APPROVE_TOKEN}}", $"{approveToken}")
            .Replace("{{POSITIVE_FEEDBACK}}", "Approve");
        
        await _emailService.SendEmailWithAttachments(user.Email, "Alliance Software Inc. Applicant References", body, pdfs);
    }

    public async Task SendReferenceListReminder(string email, string fullname,
        List<CharacterReference> noReplyReferences, string role)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);

        var bodyBuilder = new StringBuilder();
        bodyBuilder.Append($"Dear {fullname},<br>" +
                           $"<br> It's sad to inform you that some of your references did not answer the form within 48 hours.<br>" +
                           $"<br> These are the lists: <br>");
        
        foreach (var reference in noReplyReferences)
        {
            // Append the reference name to the email body
            bodyBuilder.Append($"- {reference.Name}<br>");
        }

        // Append closing message
        bodyBuilder.Append("<br>If you have alternative reference contacts, please provide them within the next 12 hours through email.<br>");

        // Create a StringBuilder to efficiently build the email body
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "List of References who did not answer the form")
            .Replace("{{BODY}}", bodyBuilder.ToString());

        // Loop through the noReplyReferences list and get their names
       
        // Complete the email body and send the email
        await _emailService.SendEmail(email, role, body);
    }


    /// <summary>
    ///     Sends automated reminders to users based on their roles and assigned tasks.
    ///     This method will be executed every two weeks to remind users of their pending tasks.
    /// </summary>
    public async Task SendAutomatedReminder()
    {
        // Implement the logic for HR to plot their schedules
        // This method will be executed every two weeks
        var allUser = _userRepository.RetrieveAll();
        foreach (var user in allUser)
            if (user.Role == "Human Resources")
            {
                //Notify Interviewer for their Task
                var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
                var templateContent = File.ReadAllText(templatePath);
                var body = templateContent
                    .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                    .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                    .Replace("{{HEADLINE}}", "Reminder to the Human Resource Team")
                    .Replace("{{BODY}}", $"Dear {user.Username},<br>" +
                                         $"<br> Human Resource team do plot your schedules.<br>" +
                                         $"Please do access your account to see the details in the website");

                await _emailService.SendEmail(user.Email, "Reminder for the Human Resource Team", body);
            }
            else
            {
                //Notify Interviewer for their Task
                var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
                var templateContent = File.ReadAllText(templatePath);
                var body = templateContent
                    .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
                    .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
                    .Replace("{{HEADLINE}}", "Reminder to the assigned Interviewer")
                    .Replace("{{BODY}}", $"Dear {user.Username},<br>" +
                                         $"<br> Reminder! You have a assigned job position to interview.<br>" +
                                         $"Please do access your account to see the details in the website");

                await _emailService.SendEmail(user.Email, "Reminder for the interviewer", body);
            }
    }

    /// <summary>
    ///     Sends an email to an applicant containing their Unique Tracking ID.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public async Task SendGUIDEmail(Guid applicationId, Applicant applicant)
    {
        //Notify Applicant for their Unique Tracking ID
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Application Status Tracker")
            .Replace("{{BODY}}", $"Dear {applicant.Firstname},<br>" +
                                 $"<br> We are reaching out to provide you with your application ID for tracking purposes. Please find the details below:<br>" +
                                 $"<br> <b>Application ID: [{applicationId}]</b> <br>" +
                                 $"<br> This unique ID will allow you to track the progress of your application. To do so, please follow these steps: <br>" +
                                 $"<br> 1. Visit Alliance Software Inc. website at [website URL]." +
                                 $"<br> 2. Navigate to the application tracker section." +
                                 $"<br> 3. Enter your Application ID: {applicationId}" +
                                 $"<br> 4. Click the \"Track\" button. <br>" +
                                 $"<br> The tracking system will provide real-time updates on the status of your application, from submission to evaluation. We appreciate your patience throughout the process. <br>" +
                                 $"<br> If you have any questions or encounter any issues, please reach out to our support team at [contact email/phone number]. <br>" +
                                 $"<br> Thank you for choosing Alliance Software Inc. We wish you the best of luck with your application!");

        await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Application Status Tracker", body);
    }

    /// <summary>
    ///     Sends the status notification.
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
            .Replace("{{BODY}}",
                $"Dear {applicant.Firstname},<br> Applicant [{applicant.Id}] has changed its status. <br> Current Status: {newStatus}");

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
        Dictionary<string, string> tokenClaims = new Dictionary<string, string>()
            {
                { "action", "ChangeStatus" },
                { "userId", user.Id.ToString() },
                { "appId", appId.ToString() },
                { "newStatus", newStatus },
                { "choice", "approved" },
            };
        string approveToken = _tokenHelper.GenerateToken(tokenClaims);

        tokenClaims["choice"] = "rejected";
        string rejectToken = _tokenHelper.GenerateToken(tokenClaims);

        newStatus = newStatus.Replace("For ", "");

        var templatePath = Path.Combine("wwwroot", "template", "ApprovalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Approval Email")
            .Replace("{{REJECT_TOKEN}}", $"{rejectToken}")
            .Replace("{{APPROVE_TOKEN}}", $"{approveToken}")
            .Replace("{{BODY}}", $"<br> Dear {user.Fullname}," +
                        $"<br> We would like to request your input regarding the current status of the applicant, {applicant.Firstname} {applicant.Lastname}, " +
                        $"in the {newStatus} phase of the hiring process." +
                        $"<br><br> Applicant ID: {applicant.Id} <br> Applicant Name: {applicant.Firstname} {applicant.Lastname}" +
                        $"<br><br> Please click the Pass button if the applicant has successfully passed the {newStatus}. Otherwise, click Fail if the applicant " +
                        $"did not meet the criteria for progressing. Thank you. <br><br>")
            .Replace("{{NEGATIVE_FEEDBACK}}", "Fail")
            .Replace("{{POSITIVE_FEEDBACK}}", "Pass");

        await _emailService.SendEmail(user.Email, "Alliance Software Inc. Applicant Status Update", body);
    }

    /// <summary>
    ///     Sends the rejected email.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="newStatus">The new status.</param>
    public async Task SendRejectedEmail(Applicant applicant, string newStatus)
    {
        var redirectLink = "https://localhost:50341/Home";
        await _emailService.SendEmail(applicant.Email, "Applicant Status Update for Applicant",
            $"<b>Dear {applicant.Firstname},</b> <br><br> Thank you for submitting your application. We have received it successfully and appreciate your interest in joining our team. <br><br> <b>Status:</b> {newStatus}. " +
            $"<br><br> <em>This is an automated messsage. Do not reply</em> <br><br> <a href=\"{redirectLink}\" " +
            $"style=\"background-color: #FF0000; border: none; color: white; padding: 10px 24px; text-align: center; text-decoration: underline; " +
            $"display: inline-block; font-size: 14px; margin: 4px 2px; cursor: pointer;\">Visit Alliance</a>");
    }

    /// <summary>
    ///     Sends the regret email.
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
                                 $"<br> After carefully reviewing your application, we regret to inform you that your profile does not align closely with the specific qualifications and requirements we are seeking for this role. <br>" +
                                 $"<br> Please know that this decision does not reflect on your capabilities or potential. We recognize the effort you put into your application, and we sincerely appreciate your interest in joining our team. <br>" +
                                 $"<br> We encourage you to continue exploring other opportunities within our organization or elsewhere that may be a better fit for your skills and aspirations. <br>" +
                                 $"<br> Once again, thank you for your interest in Alliance Software Inc. We wish you all the best in your job search and future endeavors. <br>" +
                                 $"<br> If you have any questions or would like feedback on your application, please feel free to reach out. We're more than willing to provide insights that may be helpful in your career journey. <br>" +
                                 $"<br> Best regards,");

        await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Application Status Update", body);
    }

    /// <summary>
    /// Sends the schedule to applicant.
    /// </summary>
    public async Task SendScheduleToApplicant(UserSchedule userSchedule, Applicant applicant, int tokenExpiry)
    {
        Dictionary<string, string> tokenClaims = new Dictionary<string, string>()
            {
                { "action", "AcceptSchedule" },
                { "userScheduleId", userSchedule.Id.ToString() },
            };
        string acceptToken = _tokenHelper.GenerateToken(tokenClaims, tokenExpiry);

        tokenClaims["action"] = "RejectSchedule";
        string rejectToken = _tokenHelper.GenerateToken(tokenClaims, tokenExpiry);

        var baseUrl = "https://localhost:50341";
        var acceptUrl = $"{baseUrl}/Scheduler/AcceptSchedule/{HttpUtility.UrlEncode(acceptToken)}";
        var rejectUrl = $"{baseUrl}/Scheduler/RejectSchedule/{HttpUtility.UrlEncode(rejectToken)}";

        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", $"{userSchedule.Type} Schedule")
            .Replace("{{BODY}}", $"<br> Dear {applicant.Firstname} {applicant.Lastname}," +
                                 $"<br><br> Your {userSchedule.Type} has been scheduled for {userSchedule.Schedule.ToShortDateString()} at {userSchedule.Schedule.ToShortTimeString()}." +
                                 $"<br><br> If you are available on the said schedule, please click the Accept button. Otherwise, click the Reject button." +
                                 $"<br><br> If you reject, the interviewer will be informed, and a new schedule will be set for you." +
                                 $"<br><br> Please click the button below that corresponds to your choice:" +
                                 $"<br><br> <a style=\"margin-right: 20px;\" href=\"{acceptUrl}\">Accept</a> " +
                                 $"<a href=\"{rejectUrl}\">Reject</a>" +
                                 $"<br><br> Best regards,");

        await _emailService.SendEmail(applicant.Email, $"Alliance Software Inc. {userSchedule.Type} Schedule", body);
    }

    /// <summary>
    ///     Sends the schedules to the interviewer.
    /// </summary>
    public async Task SendSchedulesToInterviewer(string fullname, string userEmail, string jobOpeningTitle,
        DateOnly date, string scheduledTimes, string meetingType)
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

    /// <summary>
    ///     Sends a request for a character reference to the specified reference for a particular applicant.
    /// </summary>
    /// <param name="reference">The character reference object containing information about the reference.</param>
    /// <param name="applicant">The applicant object containing information about the applicant.</param>
    /// <param name="userId">The ID of the user making the request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SendRequestReference(CharacterReference reference, Applicant applicant, int userId)
    {
        var url = $"https://localhost:50341/BackgroundCheck/Form/{reference.Id}/{userId}";
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Character Reference")
            .Replace("{{BODY}}", $"Dear {reference.Name},<br>" +
                                 $"<br> I hope this email finds you well. We appreciate your willingness to provide a character reference for applicant, {applicant.Firstname} {applicant.Lastname}.<br/>" +
                                 $"<br> To proceed with the application process, we kindly request you to complete the Character Reference Form by following the link below:<br/>" +
                                 $"<br> <a href=\"{url}\">Character Reference Form</a> ");

        await _emailService.SendEmail(reference.Email, "Character Reference Form Request", body);
    }

    /// <summary>
    ///     Sends the gratitude email.
    /// </summary>
    /// <param name="applicant">The applicant.</param>
    /// <param name="reference">The reference.</param>
    public async Task SendGratitudeEmail(Applicant applicant, BackgroundCheck reference)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Character Reference Form Completion")
            .Replace("{{BODY}}", $"Dear Mr/Ms. {reference.Lastname},<br>" +
                                 $"<br> I hope this email finds you well. I wanted to extend my heartfelt appreciation for taking the time to complete the Character Reference Form on behalf of Mr/Ms. {applicant.Firstname} {applicant.Lastname}. <br>" +
                                 $"<br> Your thoughtful and positive feedback has been received and will play a crucial role in the evaluation process of his/her application. Your willingness to vouch for Mr/Ms. {applicant.Lastname}'s character speaks volumes about your relationship with them, and it is a testament to their strengths and character. <br>" +
                                 $"<br> If you have any questions or need any further information about the application process, please do not hesitate to reach out to us. <br>" +
                                 $"<br> Once again, thank you for being a part of Mr/Ms. {applicant.Lastname}'s journey through this process. We truly appreciate your contribution to our decision-making process. <br>" +
                                 $"<br> Best regards,");

        await _emailService.SendEmail(reference.Email, "Alliance Software Inc. Background Check", body);
    }

    /// <summary>
    ///     Informs the interviewer that a schedule has been rejected.
    /// </summary>
    public async Task SendRejectedScheduleNoticeToInterviewer(string email, string fullname, UserSchedule userSchedule,
        string applicantFullName)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", $"Applicant {userSchedule.Type} Schedule Rejected")
            .Replace("{{BODY}}", $"Dear {fullname},<br>" +
                                 $"<br> We hope this email finds you well. We would like to inform you that the {userSchedule.Type} schedule that you set for Mr./Ms. {applicantFullName}" +
                                 $" on {userSchedule.Schedule} has been rejected. Please reschedule the meeting through our HR Automation System. <br> " +
                                 $"<br> To set a new schedule, please follow these steps: <br> 1. Log in to the HR Automation System using your credentials. <br> 2. Navigate to the HR Scheduler." +
                                 $"<br> 3. Select the appropriate job opening, meeting type, and date. <br> 4. Add the applicant(s) you want to set a schedule for." +
                                 $"<br> 5. Select a time for each applicant. <br> 6. Click the submit button. <br>" +
                                 $"<br> Should you encounter any difficulties or require any support in the process, please do not hesitate to reach out to our support team, and we will be glad to assist you. <br>" +
                                 $"<br><br> Best regards,");

        await _emailService.SendEmail(email,
            $"Alliance Software Inc. {userSchedule.Type} Schedule Rejection Notification", body);
    }

    /// <summary>
    ///     Sends the accepted schedule with Teams link to the interviewer.
    /// </summary>
    public async Task SendAcceptedScheduleToInterviewer(string email, string fullname, UserSchedule userSchedule,
        ApplicationViewModel application, string joinUrl)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", $"Applicant {userSchedule.Type} Schedule Accepted")
            .Replace("{{BODY}}", $"<br> Dear {fullname}," +
                                 $"<br><br> Warm greetings! We are delighted to inform you that an applicant has accepted the {userSchedule.Type} schedule you proposed through our HR Automation System." +
                                 $"<br><br> The details for the scheduled {userSchedule.Type} are as follows: <br> Applicant: {application.ApplicantName}" +
                                 $"<br> Position: {application.JobOpeningTitle} <br> Date: {userSchedule.Schedule.ToShortDateString()} <br> Time: {userSchedule.Schedule.ToShortTimeString()} <br> Meeting Link: {joinUrl}" +
                                 $"<br><br> With the provided Teams Meeting link, you and the applicant can effortlessly join the virtual meeting at the scheduled time. Please ensure you have access to a " +
                                 $"stable internet connection and a working camera and microphone for a seamless meeting experience." +
                                 $"<br><br>Should any unforeseen circumstances arise or if you encounter any challenges with the virtual meeting link, please reach out to our support team, and we will be more than happy to assist you." +
                                 $"<br><br><br> Best regards,");

        await _emailService.SendEmail(email,
            $"Alliance Software Inc. {userSchedule.Type} Schedule Acceptance Notification", body);
    }

    public async Task SendAcceptedScheduleToApplicant(string email, UserSchedule userSchedule,
        ApplicationViewModel application, string joinUrl)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", $"{userSchedule.Type} Schedule Accepted")
            .Replace("{{BODY}}", $"<br> Dear {application.ApplicantName}," +
                                 $"<br><br> Warm greetings! Thank you for accepting the {userSchedule.Type} schedule." +
                                 $"<br><br> The details for the scheduled {userSchedule.Type} are as follows: " +
                                 $"<br> Position: {application.JobOpeningTitle} <br> Date: {userSchedule.Schedule.ToShortDateString()} <br> Time: {userSchedule.Schedule.ToShortTimeString()} <br> Meeting Link: {joinUrl}" +
                                 $"<br><br> You can now look forward to your scheduled interview at the designated time. We will be utilizing Microsoft Teams for the virtual interview, so please ensure " +
                                 $"you are familiar with the platform and have access to a stable internet connection. Additionally, make sure your camera and microphone are in good working order to " +
                                 $"facilitate a smooth interview experience." +
                                 $"<br><br>If you have any queries or require further assistance regarding the interview process or the Teams Meeting link, please feel free to contact us. We are here to support " +
                                 $"you and ensure that your interview experience is both positive and successful." +
                                 $"<br><br><br> Best regards,");

        await _emailService.SendEmail(email,
            $"Alliance Software Inc. {userSchedule.Type} Schedule Acceptance Notification", body);
    }

    /// <summary>
    ///     Sends the password change.
    /// </summary>
    /// <param name="userEmail">The user email.</param>
    /// <param name="callBackUrl">The call back URL.</param>
    public async Task SendPasswordChange(string userEmail, string callBackUrl)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Forgot Password Link")
            .Replace("{{BODY}}", $"You have requested to change your password click this link below" +
                                 $"<br><br><div style=\"display:flex; align-items:center; justify-content:center\"><a href=\"{callBackUrl}\" style=\"background-color: #FF0000;" +
                                 $"border: none; color: white; padding: 10px 24px; text-align: center; text-decoration: underline; border-radius:5px;" +
                                 $"font-size: 14px; margin: 4px 2px; cursor: pointer;\">CHANGE PASSWORD</a></div>");

        await _emailService.SendEmail(userEmail, "Alliance Software Inc. Forgot Password", body);
    }

    /// <summary>
    /// Sends the congratulations to applicant.
    /// </summary>
    public async Task SendCongratulation(Applicant applicant, int userId)
    {
        Dictionary<string, string> tokenClaims = new Dictionary<string, string>()
            {
                { "action", "AcceptOffer" },
                { "userId", userId.ToString() },
            };
        string acceptToken = _tokenHelper.GenerateToken(tokenClaims);

        tokenClaims["action"] = "RejectOffer";
        string rejectToken = _tokenHelper.GenerateToken(tokenClaims);

        var baseUrl = "https://localhost:50341";
        var acceptUrl = $"{baseUrl}/CurrentHire/AcceptOffer/{HttpUtility.UrlEncode(acceptToken)}";
        var rejectUrl = $"{baseUrl}/CurrentHire/RejectOffer/{HttpUtility.UrlEncode(rejectToken)}";

        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Job Offer")
            .Replace("{{BODY}}", $"Dear Mr/Ms. {applicant.Lastname},<br>" +
                                 $"<br> Congratulations. You're hired Mr/Ms. {applicant.Firstname} {applicant.Lastname}. <br>" +
                                 $"<br> If you have any questions or need any further information about the company details, please do not hesitate to reach out to us. <br>" +
                                 $"<br> Once again, congratulations. We truly appreciate your decision for choosing the alliance software inc." +
                                 $"<br> Best regards, <br/>" +
                                 $"<br> Please click the button to accept or reject for being hired<br/>" +
                                 $"<br> <a href=\"{acceptUrl}\">Accept</a> " +
                                 $"<a href=\"{rejectUrl}\">Reject</a>");

        await _emailService.SendEmail(applicant.Email, "Alliance Software Inc. Job Offer", body);
    }

    public async Task SendRejectedHireNoticeToInterviewer(string email, string fullname, string position,
        CurrentHire currentHire, string applicantFullName)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Update on Your Job Application with Alliance Software Inc")
            .Replace("{{BODY}}", $"Dear {fullname},<br>" +
                                 $"<br> I hope this email finds you well. I wanted to take a moment to personally thank you for your interest in the {position} role at Alliance Software Inc. " +
                                 $" We truly appreciate the time and effort you invested in the application and interview process. <br> " +
                                 $"<br> We wish you all the best in your job search and professional endeavors. Should you have any questions or need any " +
                                 $"<br> feedback on your application or interview performance, please feel free to reach out to us.<br>" +
                                 $"<br> Thank you once again for considering [Company Name] as your potential employer. We " +
                                 $"<br> genuinely appreciate your interest, and we hope our paths cross again in the future. <br>" +
                                 $"<br><br> Best regards,");

        await _emailService.SendEmail(email, $"Alliance Software Inc!", body);
    }

    /// <summary>
    /// Sends the exam score reminder.
    /// </summary>
    public async Task SendExamScoreReminder(string email, string fullname, ApplicationViewModel application)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", $"Reminder: Input Applicant's Exam Score")
            .Replace("{{BODY}}", $"<br> Dear {fullname}," +
                                 $"<br><br> We kindly remind you to input the exam score for {application.ApplicantName} who recently underwent the Technical Exam phase for the position of {application.JobOpeningTitle}." +
                                 $"<br><br> To input the applicant's score, please follow these steps: <br> 1. Log in to the HR Automation System using your credentials. <br> 2. Navigate to the Directory." +
                                 $"<br> 3. Click the View button of the job opening applied for by the applicant. <br> 4. Click the Exams tab. <br> 5. Click the Input Score button for the applicant." +
                                 $"<br> 6. Input the applicant's score and the perfect score in the modal." +
                                 $"<br><br> Should you encounter any difficulties or require any support in the process, please do not hesitate to reach out to our support team, and we will be glad to assist you. <br>" +
                                 $"<br><br> Best regards,");

        await _emailService.SendEmail(email, $"Alliance Software Inc. Applicant Exam Score Reminder", body);
    }

    /// <summary>
    /// Sends notification to HR if a reference has successfully answered a form.
    /// </summary>
    /// <param name="reference">The reference.</param>
    /// <param name="user">The user.</param>
    /// <param name="applicant">The applicant.</param>
    /// <returns></returns>
    public async Task SendBackgroundCheckCompletionToHR(BackgroundCheck reference, User user, Applicant applicant)
    {
        var templatePath = Path.Combine("wwwroot", "template", "FormalEmail.html");
        var templateContent = File.ReadAllText(templatePath);
        var body = templateContent
            .Replace("{{HEADER_LINK}}", "https://zimmergren.net")
            .Replace("{{HEADER_LINK_TEXT}}", "HR Automation System")
            .Replace("{{HEADLINE}}", "Character Reference Form Completion")
            .Replace("{{BODY}}", $"Dear {user.Fullname},<br>" +
                                 $"<br> {reference.Firstname} {reference.Lastname} has successfully answered the Character Reference Form for Applicant [{applicant.Id}] {applicant.Firstname} {applicant.Lastname}.");

        await _emailService.SendEmail(user.Email, "Alliance Software Inc. Background Check", body);
    }
}
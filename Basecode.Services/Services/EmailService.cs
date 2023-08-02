using System.Net;
using System.Net.Mail;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class EmailService : IEmailService
{
    /// <summary>
    ///     Sends an email using the specified SMTP client with the provided recipient, subject, and body.
    /// </summary>
    /// <param name="recipient">The email address of the recipient.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <returns>
    ///     A task representing the asynchronous operation.
    /// </returns>
    public async Task SendEmail(string recipient, string subject, string body)
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
}
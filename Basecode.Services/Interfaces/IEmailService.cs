namespace Basecode.Services.Interfaces;

public interface IEmailService
{
    /// <summary>
    ///     Sends the email.
    /// </summary>
    /// <param name="recipient">The recipient.</param>
    /// <param name="subject">The subject.</param>
    /// <param name="body">The body.</param>
    /// <returns></returns>
    Task SendEmail(string recipient, string subject, string body);

    Task SendEmailWithAttachments(string recipient, string subject, string body, List<byte[]> pdfs);
}
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpSettings = _configuration.GetSection("SmtpSettings");

        using (var client = new SmtpClient(smtpSettings["Server"]))
        {
            client.Port = int.Parse(smtpSettings["Port"]);
            client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
            client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
            client.UseDefaultCredentials = false;

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = false,
            };

            mailMessage.To.Add(email);

            try
            {
                _logger.LogInformation("Sending email to {Email} with subject {Subject}", email, subject);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP Exception: Could not send email to {Email}. Server response: {StatusCode}", email, ex.StatusCode);
                throw new InvalidOperationException("Could not send email due to SMTP error.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception: Could not send email to {Email}", email);
                throw new InvalidOperationException("An error occurred while sending email.", ex);
            }
        }
    }
}
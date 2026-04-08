using QuartzAPI.Models;

namespace QuartzAPI.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toAddress, string subject, string body, bool isHtml = true);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toAddress, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var senderEmail = _configuration["Email:SenderEmail"] ?? "your-email@gmail.com";
            var senderPassword = _configuration["Email:SenderPassword"] ?? "your-app-password";

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(senderEmail, senderPassword);

                var message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress("Quartz Scheduler", senderEmail));
                message.To.Add(new MimeKit.MailboxAddress("", toAddress));
                message.Subject = subject;

                message.Body = new MimeKit.TextPart(isHtml ? "html" : "plain")
                {
                    Text = body
                };

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {toAddress}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
            throw;
        }
    }
}

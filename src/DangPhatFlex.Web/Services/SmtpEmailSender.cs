using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DangPhatFlex.Web.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body)
    {
        var host = _configuration["Smtp:Host"];
        if (string.IsNullOrEmpty(host))
        {
            _logger.LogWarning("Smtp:Host not configured; skipping email send to {ToEmail}", toEmail);
            return;
        }

        using var client = new SmtpClient(host, int.Parse(_configuration["Smtp:Port"] ?? "587"))
        {
            Credentials = new NetworkCredential(_configuration["Smtp:User"], _configuration["Smtp:Password"]),
            EnableSsl = true
        };

        using var message = new MailMessage(_configuration["Smtp:From"] ?? "no-reply@dangphatflex.vn", toEmail, subject, body);
        await client.SendMailAsync(message);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ScoutAsset.Server.Application.Services;
using System.Net;
using System.Net.Mail;

namespace ScoutAsset.Server.Infrastructure.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _config["Smtp:Host"] ?? "localhost";
        var portStr = _config["Smtp:Port"] ?? "25";
        var enableSslStr = _config["Smtp:EnableSsl"] ?? "false";
        var username = _config["Smtp:Username"] ?? string.Empty;
        var password = _config["Smtp:Password"] ?? string.Empty;
        var fromAddress = _config["Smtp:FromAddress"] ?? (string.IsNullOrEmpty(username) ? "noreply@scoutinventory.com" : username);
        var fromName = _config["Smtp:FromName"] ?? "Scout Inventory";

        int.TryParse(portStr, out var port);
        bool.TryParse(enableSslStr, out var enableSsl);

        _logger.LogInformation("Sending email to {Email} with subject {Subject} via SMTP host {Host}", email, subject, host);

        try
        {
            using var mail = new MailMessage();
            mail.From = new MailAddress(fromAddress, fromName);
            mail.To.Add(email);
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;

            using var smtp = new SmtpClient(host, port);
            smtp.EnableSsl = enableSsl;

            if (!string.IsNullOrEmpty(username))
            {
                smtp.Credentials = new NetworkCredential(username, password);
            }

            await smtp.SendMailAsync(mail);
            _logger.LogInformation("Email sent successfully to {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email} via SMTP", email);
            // Do not throw to avoid crashing parent transaction if SMTP is not configured
        }
    }
}

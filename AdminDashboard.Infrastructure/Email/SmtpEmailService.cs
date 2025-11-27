using System.Net;
using System.Net.Mail;
using AdminDashboardApplication.Interfaces;
using Microsoft.Extensions.Options;

namespace AdminDashboard.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings.SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings.SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        // Setup SMTP client
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl, 
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        // Build proper From address (with optional display name)
        var fromAddress = string.IsNullOrWhiteSpace(_settings.FromName)
            ? new MailAddress(_settings.From)
            : new MailAddress(_settings.From, _settings.FromName);

        // Build email
        using var mail = new MailMessage
        {
            From = fromAddress,
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        // Send
        await client.SendMailAsync(mail);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
    {
        // Setup SMTP client
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        // Build proper From address (with optional display name)
        var fromAddress = string.IsNullOrWhiteSpace(_settings.FromName)
            ? new MailAddress(_settings.From)
            : new MailAddress(_settings.From, _settings.FromName);

        // Build email
        using var mail = new MailMessage
        {
            From = fromAddress,
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        // Add PDF attachment
        if (attachmentBytes != null && attachmentBytes.Length > 0)
        {
            var stream = new MemoryStream(attachmentBytes);
            var attachment = new Attachment(stream, attachmentFileName, "application/pdf");
            mail.Attachments.Add(attachment);
        }

        // Send
        await client.SendMailAsync(mail);
    }
}
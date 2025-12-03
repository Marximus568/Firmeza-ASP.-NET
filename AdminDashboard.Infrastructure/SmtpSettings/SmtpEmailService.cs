using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace AdminDashboard.Infrastructure.SmtpSettings;

public class SmtpEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        // STARTTLS mode (port 587)
        if (_settings.UseStartTls)
        {
            client.EnableSsl = true; // Required for STARTTLS handshake
        }

        // SSL direct mode (port 465)
        if (_settings.EnableSsl && !_settings.UseStartTls)
        {
            client.EnableSsl = true;
        }

        var fromAddress = new MailAddress(
            _settings.From,
            string.IsNullOrWhiteSpace(_settings.FromName) ? null : _settings.FromName
        );

        using var mail = new MailMessage
        {
            From = fromAddress,
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await client.SendMailAsync(mail);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        if (_settings.UseStartTls)
        {
            client.EnableSsl = true;
        }

        if (_settings.EnableSsl && !_settings.UseStartTls)
        {
            client.EnableSsl = true;
        }

        var fromAddress = new MailAddress(
            _settings.From,
            string.IsNullOrWhiteSpace(_settings.FromName) ? null : _settings.FromName
        );

        using var mail = new MailMessage
        {
            From = fromAddress,
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        // Add attachment
        if (attachmentBytes != null && attachmentBytes.Length > 0)
        {
            var stream = new MemoryStream(attachmentBytes);
            var attachment = new Attachment(stream, attachmentFileName, "application/pdf");
            mail.Attachments.Add(attachment);
        }

        await client.SendMailAsync(mail);
    }
}

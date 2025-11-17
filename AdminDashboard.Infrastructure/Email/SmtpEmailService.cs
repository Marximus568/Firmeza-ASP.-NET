using System.Net;
using System.Net.Mail;
using AdminDashboard.Domain.Interfaces;
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
        var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_settings.Username, _settings.Password)
        };

        var mail = new MailMessage(_settings.From, to, subject, body)
        {
            IsBodyHtml = true // si usas HTML
        };

        await client.SendMailAsync(mail);
    }
}
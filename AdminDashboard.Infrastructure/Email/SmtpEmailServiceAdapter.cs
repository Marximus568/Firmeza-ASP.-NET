using AdminDashboardApplication.Interfaces;

namespace AdminDashboard.Infrastructure.Email;

public class SmtpEmailServiceAdapter : IEmailService
{
    private readonly SmtpSettings.SmtpEmailService _smtpEmailService;

    public SmtpEmailServiceAdapter(SmtpSettings.SmtpEmailService smtpEmailService)
    {
        _smtpEmailService = smtpEmailService;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        await _smtpEmailService.SendEmailAsync(to, subject, body);
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
    {
        await _smtpEmailService.SendEmailWithAttachmentAsync(to, subject, body, attachmentBytes, attachmentFileName);
    }
}

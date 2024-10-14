using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace FinesRegister.Models;

public class EmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string recipient, string subject, string message)
    {
        var smtpClient = new SmtpClient(_smtpSettings.Host)
        {
            Port = _smtpSettings.Port,
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSsl,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Username),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(recipient);

        await smtpClient.SendMailAsync(mailMessage);
    }
}

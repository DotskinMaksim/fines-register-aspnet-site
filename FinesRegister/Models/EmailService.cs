using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace FinesRegister.Models
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private Dictionary<string, string> messages;
        private Dictionary<string, string> subjects;


        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
            messages = new Dictionary<string, string>
            {
                {"confirmationLink", $@"
                <h1>Kinnitage registreerimine</h1>
                <p>Konto kinnitamiseks klõpsake alloleval nupul:</p>
                <a href='{{confirmationLink}}' style='display:inline-block; padding: 10px 20px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px;'>Kinnita konto</a>
                "},
                
                {"confirmationCode", $@"
                <h1>Kinnitage registreerimine</h1>
                <p>Teie kinnituskood: <strong>{{confirmationCode}}</strong></p>
                <p>Sisestage see kood vastavasse väljale veebisaidil, et registreerimine lõpetada.</p>
                <br>
                <p>Kui te ei ole registreerimist taotlenud, siis võite selle kirja ignoreerida.</p> 
                
                "}
            };
           
        }
        public string GenerateRandomNumbers(int length)
        {
            Random random = new Random();
            int[] randomNumbers = new int[6];
            string code = "";

            for (int i = 0; i < length; i++)
            {
                randomNumbers[i] = random.Next(0, 10);
            }

            foreach (var number in randomNumbers)
            {
                code+=number.ToString();
            }

            return code;
        }

        public async Task SendConfirmationEmail(string toEmail, string confirmationElement, string messageKey, string messageSubject)
        {
            var messageBody = messages[messageKey].Replace("{"+messageKey+"}", confirmationElement);

            var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.Username),
                Subject = messageSubject,
                IsBodyHtml = true, 
                Body = messageBody,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

       
    }
}

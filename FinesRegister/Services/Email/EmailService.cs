using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using FinesRegister.Models;
// C1HVXWDE76UWFUPVZ1VTGUZW

namespace FinesRegister.Services.Email
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private Dictionary<string, string> subjects;


        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
           
           
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

        public async Task SendConfirmationEmail(string toEmail, string messageKey, string messageSubject,
            string confirmationElement = null, Fine fine = null) {
            
            
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Services/Email/EmailTemplates", messageKey+".html");

            string messageBody = await File.ReadAllTextAsync(templatePath);

            switch (messageKey)
            {
                case "ConfirmationLink" or "ConfirmationCode":
                    messageBody = messageBody.Replace("{{confirmationCode}}", confirmationElement);
                    break;
                case "FineNotification":
                    if (fine == null)
                    {
                        throw new ArgumentNullException(nameof(fine), "Fine cannot be null for fine notification");
                    }

                    messageBody = fine.FormatMessageBody(messageBody);
                    break;

            }


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

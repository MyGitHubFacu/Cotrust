using Cotrust.Intefaces;
using System.Net;
using System.Net.Mail;

namespace Cotrust.Classes
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string body)
        {
            var mail = "facundosola11@hotmail.com";
            var pw = "kpv0slender";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw),
                UseDefaultCredentials = false,
            };
            
            return client.SendMailAsync(new MailMessage(mail, email, subject, body));
        }
    }
}

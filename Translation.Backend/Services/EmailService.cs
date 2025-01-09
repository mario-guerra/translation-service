using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace AudioTranslationService.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Audio Translation Service", "no-reply@audiotranslationservice.com"));
            email.To.Add(new MailboxAddress(to, to));
            email.Subject = subject;
            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                // For demo purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("smtp.example.com", 587, false);
                await client.AuthenticateAsync("username", "password");
                await client.SendAsync(email);
                await client.DisconnectAsync(true);
            }
        }
    }
}
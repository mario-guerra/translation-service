using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.Communication.Email;

namespace AudioTranslationService.Services
{
    public class EmailService
    {
        private readonly EmailClient _emailClient;
        private readonly string _senderAddress;
        private readonly string _baseUrl;

        public EmailService(string connectionString, string senderAddress, string baseUrl)
        {
            _senderAddress = senderAddress;
            _emailClient = new EmailClient(connectionString);
            _baseUrl = baseUrl;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = new EmailMessage(
                senderAddress: _senderAddress,
                content: new EmailContent(subject)
                {
                    PlainText = body,
                    Html = $@"
                    <html>
                        <body>
                            <p><strong>Invoice:</strong></p>
                            <pre>{body}</pre>
                        </body>
                    </html>"
                },
                recipients: new EmailRecipients(new List<EmailAddress> { new EmailAddress(to) })
            );

            try
            {
                EmailSendOperation emailSendOperation = await _emailClient.SendAsync(
                    WaitUntil.Completed,
                    emailMessage
                );

                if (emailSendOperation.HasCompleted)
                {
                    Console.WriteLine($"Email sent successfully to {to}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

        public string GenerateDownloadLink(string containerName, string uploadId)
        {
            var downloadLink = _baseUrl + $"/download?containerName={containerName}&uploadId={uploadId}";
            return downloadLink;      
            }
    }
}
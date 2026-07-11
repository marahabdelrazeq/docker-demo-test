using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace CommonLibrary.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body,
            string? filePathOrUrl = null,
            string? attachmentName = null,
            byte[]? attachmentBytes = null)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");

                string fromMail = emailSettings["FromMail"];
                string password = emailSettings["Password"];
                string host = emailSettings["Host"];
                string settingPort = emailSettings["Port"];

                int port = 0;
                if (!string.IsNullOrEmpty(settingPort))
                {
                    port = int.Parse(settingPort);
                }

                using var emailClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(fromMail, password)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(fromMail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                message.To.Add(new MailAddress(to));

                // ✅ في حالة تم تمرير byte[]
                if (attachmentBytes != null && attachmentBytes.Length > 0)
                {
                    string fileName = string.IsNullOrEmpty(attachmentName) ? "Attachment.pdf" : attachmentName;
                    var stream = new MemoryStream(attachmentBytes);
                    message.Attachments.Add(new Attachment(stream, fileName, "application/pdf"));
                }
                // ✅ أو في حالة تم تمرير مسار أو URL
                else if (!string.IsNullOrEmpty(filePathOrUrl))
                {
                    byte[] fileBytes;

                    if (filePathOrUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    {
                        using var http = new HttpClient();
                        var response = await http.GetAsync(filePathOrUrl);

                        if (!response.IsSuccessStatusCode)
                            throw new Exception($"Failed to download attachment. Status: {response.StatusCode}");

                        fileBytes = await response.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        if (!File.Exists(filePathOrUrl))
                            throw new FileNotFoundException("File not found at path: " + filePathOrUrl);

                        fileBytes = await File.ReadAllBytesAsync(filePathOrUrl);
                    }

                    string fileName = string.IsNullOrEmpty(attachmentName)
                        ? Path.GetFileName(filePathOrUrl)
                        : attachmentName;

                    var stream = new MemoryStream(fileBytes);
                    message.Attachments.Add(new Attachment(stream, fileName, "application/pdf"));
                }

                await emailClient.SendMailAsync(message);
            }
            catch
            {
                throw;
            }
        }

    }
}

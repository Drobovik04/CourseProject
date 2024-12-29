using Microsoft.Extensions.Localization;
using System.Net;
using System.Net.Mail;

namespace CourseProject.Utilities
{
    public class MailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public MailSender(IConfiguration configuration, IStringLocalizer<SharedResources> localizer)
        {
            _configuration = configuration;
            _localizer = localizer;
        }
        public async Task SendMessage(string emailReceiver, byte[] pdfBytes, byte[] htmlBytes)
        {
            SmtpClient smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(_configuration.GetValue<string>("Mail:Login"), _configuration.GetValue<string>("Mail:Password"))
            };
            MailMessage message = new MailMessage(new MailAddress(_configuration.GetValue<string>("Mail:Login"), "Info"), new MailAddress(emailReceiver))
            {
                Subject = _localizer["AnswersEmail"],
                Body = _localizer["BodyEmail"],
            };

            var htmlStream = new MemoryStream(htmlBytes);
            var pdfStream = new MemoryStream(pdfBytes);

            message.Attachments.Add(new Attachment(pdfStream, "Answers.pdf", "application/pdf"));
            message.Attachments.Add(new Attachment(htmlStream, "Answers.html", "text/html"));
            await smtp.SendMailAsync(message);
        }
    }
}

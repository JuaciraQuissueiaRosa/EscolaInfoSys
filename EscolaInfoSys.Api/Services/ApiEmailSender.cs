using System.Net.Mail;
using System.Net;
using EscolaInfoSys.Services;

namespace EscolaInfoSys.Api.Services
{
    public class ApiEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public ApiEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var mail = _configuration.GetSection("Mail");

            var email = new MailMessage
            {
                From = new MailAddress(mail["From"], mail["NameFrom"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };
            email.To.Add(toEmail);

            using var smtp = new SmtpClient(mail["Smtp"], int.Parse(mail["Port"]))
            {
                Credentials = new NetworkCredential(mail["From"], mail["Password"]),
                EnableSsl = false // ← importante para Somee
            };

            await smtp.SendMailAsync(email);
        }
    }

}

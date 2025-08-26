using EscolaInfoSys.Services;

namespace EscolaInfoSysApi.Services
{
    // Implementação “vazia” só para satisfazer o DI.
    public class NoOpEmailSender : IEmailSender
    {
       
        public Task SendAsync(string to, string subject, string html) => Task.CompletedTask;

       
        public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
    }
}

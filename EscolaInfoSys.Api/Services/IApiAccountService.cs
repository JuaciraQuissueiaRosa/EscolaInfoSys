namespace EscolaInfoSys.Api.Services
{
    public interface IApiAccountService
    {
        Task<string?> AuthenticateAsync(string email, string password);
    }
}

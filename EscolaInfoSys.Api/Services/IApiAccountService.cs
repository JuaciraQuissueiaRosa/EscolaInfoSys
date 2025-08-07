using EscolaInfoSys.Api.Models;
using EscolaInfoSys.Data;
using System.Security.Claims;

namespace EscolaInfoSys.Api.Services
{
    public interface IApiAccountService
    {
        Task<string?> AuthenticateAsync(string email, string password);
        Task<bool> SendResetPasswordEmailAsync(string email, string scheme, Func<string, string, string, string> urlBuilder);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto model);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal user);
    }

}

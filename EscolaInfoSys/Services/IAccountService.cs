using EscolaInfoSys.Data;
using EscolaInfoSys.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EscolaInfoSys.Services
{
    public interface IAccountService
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
        Task<SignInResult> PasswordSignInAsync(string username, string password);
        Task SignOutAsync();
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model, string scheme, Func<string, string, string> urlBuilder);
        Task<string?> GenerateResetPasswordLinkAsync(string email, string scheme, Func<string, string, string, string> urlBuilder);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateProfileAsync(ApplicationUser user, string fullName, IFormFile? newPhoto, string wwwRootPath);
        Task<(bool Success, string? Message)> ConfirmEmailAsync(string userId, string token);

        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<ApplicationUser?> FindByIdAsync(string userId);

    }

}

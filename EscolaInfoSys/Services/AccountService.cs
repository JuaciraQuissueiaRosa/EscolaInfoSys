using EscolaInfoSys.Data;
using EscolaInfoSys.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;

namespace EscolaInfoSys.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public Task<ApplicationUser?> FindByEmailAsync(string email) =>
            _userManager.FindByEmailAsync(email);

        public Task<bool> IsEmailConfirmedAsync(ApplicationUser user) =>
            _userManager.IsEmailConfirmedAsync(user);

        public Task<SignInResult> PasswordSignInAsync(string username, string password) =>
       _signInManager.PasswordSignInAsync(username, password, false, false);


        public Task SignOutAsync() =>
            _signInManager.SignOutAsync();

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterViewModel model, string scheme, Func<string, string, string> urlBuilder)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.FullName,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, "Student");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var confirmationLink = urlBuilder(user.Id, encodedToken);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");

            return (true, Enumerable.Empty<string>());
        }

        public async Task<string?> GenerateResetPasswordLinkAsync(string email, string scheme, Func<string, string, string, string> urlBuilder)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                return null;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return urlBuilder(token, user.Email, scheme);
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return (true, Enumerable.Empty<string>());

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }

        public Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal) =>
            _userManager.GetUserAsync(userPrincipal);

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> UpdateProfileAsync(ApplicationUser user, string fullName, IFormFile? newPhoto, string wwwRootPath)
        {
            user.Name = fullName;

            if (newPhoto != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(newPhoto.FileName);
                var filePath = Path.Combine(wwwRootPath, "uploads", fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await newPhoto.CopyToAsync(stream);
                user.ProfilePhoto = fileName;
            }

            var result = await _userManager.UpdateAsync(user);
            return (result.Succeeded, result.Errors.Select(e => e.Description));
        }

        public async Task<(bool Success, string? Message)> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return (false, "User not found");

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded) return (true, null);
            if (await _userManager.IsEmailConfirmedAsync(user)) return (true, "Already confirmed");
            return (false, "Confirmation failed");
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public Task<ApplicationUser?> FindByIdAsync(string userId) =>
        _userManager.FindByIdAsync(userId);

    }

}

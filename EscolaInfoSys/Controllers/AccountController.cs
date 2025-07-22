using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models.ViewModels;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EscolaInfoSys.Controllers
{


    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IStudentRepository _studentRepo;

        public AccountController(IAccountService accountService, IWebHostEnvironment webHostEnvironment, IStudentRepository studentRepo)
        {
            _accountService = accountService;
            _webHostEnvironment = webHostEnvironment;
            _studentRepo = studentRepo;


        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.FindByEmailAsync(model.Email);
            if (user == null || !await _accountService.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Invalid login attempt or email not confirmed.");
                return View(model);
            }

            var result = await _accountService.PasswordSignInAsync(user.UserName, model.Password);

            if (result.Succeeded)
            {
                var roles = await _accountService.GetRolesAsync(user);

                if (roles.Contains("Administrator"))
                    return RedirectToAction("Index", "FormGroups");
                if (roles.Contains("StaffMember"))
                    return RedirectToAction("Index", "Absences");
                if (roles.Contains("Student"))
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RegisterConfirmation() => View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View();

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string BuildUrl(string token, string email, string scheme) =>
                Url.Action("ResetPassword", "Account", new { token, email }, scheme);

            var emailSent = await _accountService.SendResetPasswordEmailAsync(model.Email, Request.Scheme, BuildUrl);

            if (emailSent)
                return RedirectToAction("ForgotPasswordConfirmation");


            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string token, string email) =>
            View(new ResetPasswordViewModel { Token = token, Email = email });


        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                ModelState.AddModelError("", "Invalid or corrupted token.");
                return View(model);
            }

            var result = await _accountService.ResetPasswordAsync(new ResetPasswordViewModel
            {
                Email = model.Email,
                Token = decodedToken,
                NewPassword = model.NewPassword
            });

            if (result.Succeeded)
            {
                TempData["Success"] = "Password successfully reset! You can now log in.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error);

            return View(model);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ConfirmEmailError() => View();

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);

            if (result.Success)
                return View("ConfirmEmail");

            ViewBag.Message = result.Message;
            return RedirectToAction("ConfirmEmailError");
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            var roles = await _accountService.GetRolesAsync(user);

            string? photoPath = user.ProfilePhoto;

           
            if (roles.Contains("Student"))
            {
                var student = await _studentRepo.GetByUserIdAsync(user.Id); 
                if (student != null && !string.IsNullOrEmpty(student.ProfilePhoto))
                    photoPath = student.ProfilePhoto;
            }

            var model = new ProfileViewModel
            {
                Email = user.Email,
                FullName = user.Name,
                Role = roles.FirstOrDefault(),
                ProfilePhoto = photoPath 
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _accountService.GetCurrentUserAsync(User);

            var model = new EditProfileViewModel
            {
                FullName = user.Name,
                CurrentPhoto = user.ProfilePhoto
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _accountService.GetCurrentUserAsync(User);

            var result = await _accountService.UpdateProfileAsync(user, model.FullName, model.NewPhoto, _webHostEnvironment.WebRootPath);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error);

                return View(model);
            }

            return RedirectToAction("Profile");
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _accountService.GetCurrentUserAsync(User);
            var result = await _accountService.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
                return RedirectToAction("Profile");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult SetPassword(string userId, string token) =>
            View(new SetPasswordViewModel { UserId = userId, Token = token });

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            string decodedToken;
            try
            {
                decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Token));
            }
            catch
            {
                ModelState.AddModelError("", "Invalid or corrupted token.");
                return View(model);
            }

            var result = await _accountService.ResetPasswordAsync(new ResetPasswordViewModel
            {
                Email = user.Email,
                Token = decodedToken,
                NewPassword = model.Password
            });

            if (result.Succeeded)
            {
                user.EmailConfirmed = true;
                await _accountService.UpdateProfileAsync(user, user.Name, null, _webHostEnvironment.WebRootPath);

                TempData["Success"] = "Password set successfully! You can now log in.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }
    }

}

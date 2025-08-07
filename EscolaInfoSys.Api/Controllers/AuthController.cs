using EscolaInfoSys.Api.Models;
using EscolaInfoSys.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IApiAccountService _authService;

        public AuthController(IApiAccountService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _authService.AuthenticateAsync(dto.Email, dto.Password);

            if (token == null)
                return Unauthorized(new { message = "Invalid credentials or email not confirmed." });

            return Ok(new
            {
                token,
                expiration = DateTime.UtcNow.AddHours(3)
            });
        }

        //  POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            string ResetLinkBuilder(string token, string email, string scheme)
                => $"https://www.escolainfosysapi.somee.com/reset-password?token={token}&email={email}";

            var success = await _authService.SendResetPasswordEmailAsync(dto.Email, "https", ResetLinkBuilder);

            if (!success)
                return BadRequest(new { message = "Invalid email or email not confirmed." });

            return Ok(new { message = "Reset password link sent to your email." });
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Password has been reset successfully." });
        }

        // POST: api/auth/change-password
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var user = await _authService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors });

            return Ok(new { message = "Password changed successfully." });
        }


    }

}

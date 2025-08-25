using EscolaInfoSys.Data;
using EscolaInfoSys.Models.ViewModels;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EscolaInfoSysApi.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _account;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IConfiguration _cfg;

        public AuthController(
            IAccountService account,
            UserManager<ApplicationUser> users,
            IConfiguration cfg)
        {
            _account = account;
            _users = users;
            _cfg = cfg;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginViewModel model)
        {
            // 1) valida credenciais com o teu serviço
            var signIn = await _account.PasswordSignInAsync(model.Email, model.Password);
            if (!signIn.Succeeded) return Unauthorized();

            // 2) carrega user + roles
            var user = await _account.FindByEmailAsync(model.Email);
            if (user is null) return Unauthorized();

            var roles = await _account.GetRolesAsync(user);

            // 3) emite JWT
            var token = CreateToken(user, roles);
            return Ok(new { Token = token.Token, ExpiresAtUtc = token.ExpiresAtUtc });
        }

        // POST /api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
        {
            // Envia e-mail de reset pelo teu AccountService
            // urlBuilder recebe (encodedToken, email, scheme) — vamos gerar link baseado no host atual
            var host = Request.Host.Value;
            await _account.SendResetPasswordEmailAsync(
                model.Email,
                Request.Scheme,
                (encodedToken, email, scheme) =>
                    $"{scheme}://{host}/Account/ResetPassword?token={encodedToken}&email={Uri.EscapeDataString(email)}"
            );

            // Mesmo que o e-mail não exista, devolvemos OK (não revelar existência)
            return Ok();
        }

        // POST /api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            var result = await _account.ResetPasswordAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        // POST /api/auth/change-password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var result = await _account.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        // — helpers —

        private (string Token, DateTime ExpiresAtUtc) CreateToken(ApplicationUser user, IEnumerable<string> roles)
        {
            var jwt = _cfg.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
        };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds);

            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }
    }
}
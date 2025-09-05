using EscolaInfoSys.Data;
using EscolaInfoSys.Models.ViewModels;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace EscolaInfoSysApi.API
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService _account;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IConfiguration _cfg;
        private readonly IEmailSender _email;   // <<< injeta o sender do MVC

        public AuthController(
            IAccountService account,
            UserManager<ApplicationUser> users,
            IConfiguration cfg,
            IEmailSender email)                  // <<< adiciona no ctor
        {
            _account = account;
            _users = users;
            _cfg = cfg;
            _email = email;                    // <<< guarda a instância
        }
        // POST /api/auth/login
        [AllowAnonymous]
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

        public record ForgotPasswordRequest(string Email);

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest req)
        {
            var user = await _users.FindByEmailAsync(req.Email);
            if (user is null) return Ok(); // não revelar existência

            // 1) token do Identity
            var rawToken = await _users.GeneratePasswordResetTokenAsync(user);

            // 2) codifica em Base64Url (é isto que o MVC espera como "code")
            var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));

            // 3) monta link para a página do site (NÃO use "token=", só "code=")
            var webBase = _cfg["Web:BaseUrl"] ?? "https://escolainfosys.somee.com";
            var link = $"{webBase}/Account/ResetPassword" +
                       $"?email={Uri.EscapeDataString(user.Email!)}" +
                       $"&code={code}";

            // 4) envia o e-mail
            await _email.SendEmailAsync(
                user.Email!,
                "Password reset",
                $@"<p>Clique para redefinir a sua palavra-passe:</p>
           <p><a href=""{link}"">{WebUtility.HtmlEncode(link)}</a></p>");

            // opcional: devolve o link para o app abrir direto no browser
            return Ok(new { link });
        }

        public record ResetPasswordApiRequest(string Email, string? Code, string? Token, string NewPassword);

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordApiRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.NewPassword))
                return BadRequest("Email and new password are required.");

            if (string.IsNullOrWhiteSpace(req.Code) && string.IsNullOrWhiteSpace(req.Token))
                return BadRequest("Provide either 'code' or 'token'.");

            // Se vier 'code' (do link do e-mail), decodifica para o token cru do Identity
            string tokenRaw;
            if (!string.IsNullOrWhiteSpace(req.Code))
                tokenRaw = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(req.Code));
            else
                tokenRaw = req.Token!;

            var result = await _account.ResetPasswordAsync(new ResetPasswordViewModel
            {
                Email = req.Email,
                Token = tokenRaw,
                NewPassword = req.NewPassword
            });

            if (!result.Succeeded) return BadRequest(result.Errors);
            return Ok();
        }

        // POST /api/auth/change-password
        [AllowAnonymous]
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
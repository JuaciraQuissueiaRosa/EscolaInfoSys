using EscolaInfoSys.Api.Models;
using EscolaInfoSys.Api.Services;
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
    }

}

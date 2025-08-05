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
        private readonly IApiAccountService _accountService;

        public AuthController(IApiAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _accountService.AuthenticateAsync(model.Email, model.Password);
            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new
            {
                token,
                expiration = DateTime.Now.AddHours(3)
            });
        }

      
    }

}

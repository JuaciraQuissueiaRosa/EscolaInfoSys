using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")] // Apenas administradores podem alterar
    public class SystemSettingsController : ControllerBase
    {
        private readonly ISystemSettingsRepository _settingsRepo;

        public SystemSettingsController(ISystemSettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        // GET: api/systemsettings
        [HttpGet]
        [AllowAnonymous] // Mobile pode consultar sem autenticação
        public async Task<ActionResult<SystemSettings>> Get()
        {
            var settings = await _settingsRepo.GetSettingsAsync();
            if (settings == null)
            {
                return NotFound();
            }

            return Ok(settings);
        }

        // PUT: api/systemsettings
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SystemSettings settings)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _settingsRepo.UpdateAsync(settings);
            return NoContent();
        }
    }
}

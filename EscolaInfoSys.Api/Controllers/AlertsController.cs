using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "StaffMember")]
    public class AlertsController : ControllerBase
    {
        private readonly IAlertRepository _alertRepo;
        private readonly IStaffMemberRepository _staffRepo;

        public AlertsController(IAlertRepository alertRepo, IStaffMemberRepository staffRepo)
        {
            _alertRepo = alertRepo;
            _staffRepo = staffRepo;
        }

        // POST: api/alerts
        [HttpPost]
        public async Task<IActionResult> CreateAlert([FromBody] Alert alert)
        {
            // Obtém ID do usuário logado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var staff = await _staffRepo.GetByApplicationUserIdAsync(userId);

            if (staff == null)
                return Unauthorized("Staff member not found.");

            alert.StaffId = staff.Id;
            alert.CreatedAt = DateTime.UtcNow;
            alert.IsResolved = false;

            await _alertRepo.AddAsync(alert);
            return Ok(new { message = "Alert sent successfully." });
        }

        // GET: api/alerts/my
        [HttpGet("my")]
        public async Task<IActionResult> GetMyAlerts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var staff = await _staffRepo.GetByApplicationUserIdAsync(userId);

            if (staff == null)
                return Unauthorized("Staff member not found.");

            var alerts = await _alertRepo.GetByStaffIdAsync(staff.Id);
            return Ok(alerts);
        }
    }
}

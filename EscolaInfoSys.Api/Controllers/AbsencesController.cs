using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/absences")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class AbsencesController : ControllerBase
    {
        private readonly IAbsenceRepository _absenceRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AbsencesController(
            IAbsenceRepository absenceRepo,
            IStudentRepository studentRepo,
            UserManager<ApplicationUser> userManager)
        {
            _absenceRepo = absenceRepo;
            _studentRepo = studentRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAbsences()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found.");

            var absences = await _absenceRepo.GetByStudentIdAsync(student.Id);
            return Ok(absences);
        }
    }
}

using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/marks")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class MarksController : ControllerBase
    {
        private readonly IMarkRepository _markRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarksController(
            IMarkRepository markRepo,
            IStudentRepository studentRepo,
            UserManager<ApplicationUser> userManager)
        {
            _markRepo = markRepo;
            _studentRepo = studentRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyMarks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found.");

            var allMarks = await _markRepo.GetAllAsync();
            var myMarks = allMarks
                .Where(m => m.StudentId == student.Id)
                .Select(m => new
                {
                    m.Id,
                    Subject = m.Subject?.Name,
                    m.Value,
                    m.IsPassed,
                    Date = m.Date.ToShortDateString()
                });

            return Ok(myMarks);
        }
    }
}

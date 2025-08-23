using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/staff")]
    //[Authorize]
    public class StaffController : ControllerBase
    {
        private readonly IFormGroupRepository _formGroupRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IMarkRepository _markRepo;
        private readonly IAbsenceRepository _absenceRepo;
        private readonly IStudentExclusionRepository _exclusionRepo;
        private readonly IStaffMemberRepository _staffRepo;

        private readonly UserManager<ApplicationUser> _userManager;

        public StaffController(
            IFormGroupRepository formGroupRepo,
            IStudentRepository studentRepo,
            IMarkRepository markRepo,
            IAbsenceRepository absenceRepo,
            IStudentExclusionRepository exclusionRepo,
            IStaffMemberRepository staffRepo,
            UserManager<ApplicationUser> userManager)
        {
            _formGroupRepo = formGroupRepo;
            _studentRepo = studentRepo;
            _markRepo = markRepo;
            _absenceRepo = absenceRepo;
            _exclusionRepo = exclusionRepo;
            _staffRepo = staffRepo;
            _userManager = userManager;

        }

        // GET: api/staff/students
        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var userId = _userManager.GetUserId(User);
            var staff = await _staffRepo.GetByApplicationUserIdAsync(userId);
            if (staff == null) return Unauthorized();

            var marks = await _markRepo.GetAllAsync();

            var studentIds = marks
                .Where(m => m.StaffMemberId == staff.Id)
                .Select(m => m.StudentId)
                .Distinct()
                .ToList();

            var students = await Task.WhenAll(
                studentIds
                    .Where(id => id.HasValue)
                    .Select(id => _studentRepo.GetFullByIdAsync(id.Value))
            );

            return Ok(students.Where(s => s != null));
        }




        // GET: api/staff/students/{id}/marks
        [HttpGet("students/{id}/marks")]
        public async Task<IActionResult> GetStudentMarks(int id)
        {
            var marks = await _markRepo.GetAllAsync();
            var studentMarks = marks.Where(m => m.StudentId == id);
            return Ok(studentMarks);
        }

        // GET: api/staff/students/{id}/absences
        [HttpGet("students/{id}/absences")]
        public async Task<IActionResult> GetStudentAbsences(int id)
        {
            var absences = await _absenceRepo.GetByStudentIdAsync(id);
            return Ok(absences);
        }

        // POST: api/staff/marks
        [HttpPost("marks")]
        public async Task<IActionResult> RegisterMark([FromBody] Mark mark)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _markRepo.AddAsync(mark);
            return Ok(mark);
        }

        // GET: api/staff/averages
        [HttpGet("averages")]
        public async Task<IActionResult> GetAverages()
        {
            var userId = _userManager.GetUserId(User);

            var staff = await _staffRepo.GetByApplicationUserIdAsync(userId);
            if (staff == null) return Unauthorized();

            var averages = await _markRepo.GetStudentSubjectAveragesByStaffAsync(staff.Id);
            return Ok(averages);
        }

    }
}

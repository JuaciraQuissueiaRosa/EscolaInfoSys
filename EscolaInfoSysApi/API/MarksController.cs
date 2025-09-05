using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSysApi.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MarksController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly IMarkRepository _marks;
        private readonly ISubjectRepository _subjects;

        public MarksController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            IMarkRepository marks,
            ISubjectRepository subjects)
        {
            _users = users;
            _students = students;
            _marks = marks;
            _subjects = subjects;
        }

        public record MarkDto(
            int Id,
            int SubjectId,
            string Subject,
            string EvaluationType,
            float Value,
            bool? IsPassed,
            DateTime Date
        );

        // GET /api/marks?subjectId=123 (opcional)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MarkDto>>> GetMyMarks([FromQuery] int? subjectId = null)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var all = await _marks.GetAllAsync(); // repositório existente
            var list = all.Where(m => m.StudentId == student.Id);

            if (subjectId.HasValue)
                list = list.Where(m => m.SubjectId == subjectId.Value);

            var subj = await _subjects.GetAllAsync();
            var nameById = subj.ToDictionary(s => s.Id, s => s.Name);

            var result = list
                .OrderByDescending(m => m.Date)
                .Select(m =>
                {
                    var sid = m.SubjectId ?? 0;
                    nameById.TryGetValue(sid, out var sname);
                    return new MarkDto(
                        m.Id,
                        sid,
                        sname ?? "",
                        m.EvaluationType?.ToString() ?? "",
                        m.Value,
                        m.IsPassed,
                        m.Date
                    );
                })
                .ToList();

            return Ok(result);
        }

        // GET /api/marks/averages
        // usa método já existente no IMarkRepository
        [HttpGet("averages")]
        public async Task<IActionResult> GetMyAverages()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var averages = await _marks.GetStudentSubjectAveragesAsync(student.Id);
            return Ok(averages);
        }
    }
}

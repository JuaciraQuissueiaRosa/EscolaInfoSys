using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EscolaInfoSysApi.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FeedController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly IMarkRepository _marks;
        private readonly IAbsenceRepository _absences;
        private readonly ISubjectRepository _subjects;

        public FeedController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            IMarkRepository marks,
            IAbsenceRepository absences,
            ISubjectRepository subjects)
        {
            _users = users;
            _students = students;
            _marks = marks;
            _absences = absences;
            _subjects = subjects;
        }

        public record FeedItem(
            string Type,          // "mark" | "absence"
            int Id,
            int SubjectId,
            string? Subject,      // nome da disciplina
            DateTime Date,
            float? Value,         // só para notas
            string? EvaluationType, // só para notas
            bool? IsPassed        // só para notas
        );

        // GET /api/feed?since=2025-08-01T00:00:00Z
        // Se não informar "since", usa últimos 30 dias
        [HttpGet]
        public async Task<ActionResult<object>> Get([FromQuery] DateTime? since = null)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var sinceUtc = (since?.ToUniversalTime()) ?? DateTime.UtcNow.AddDays(-30);

            // subjects para mapear nomes
            var subjectList = await _subjects.GetAllAsync();
            var subjectNameById = subjectList.ToDictionary(s => s.Id, s => s.Name);

            // notas do aluno desde 'since'
            var allMarks = await _marks.GetAllAsync();
            var myMarks = allMarks
                .Where(m => m.StudentId == student.Id && m.Date >= sinceUtc)
                .Select(m =>
                {
                    var sid = m.SubjectId ?? 0;
                    subjectNameById.TryGetValue(sid, out var sname);
                    return new FeedItem(
                        Type: "mark",
                        Id: m.Id,
                        SubjectId: sid,
                        Subject: sname,
                        Date: m.Date,
                        Value: m.Value,
                        EvaluationType: m.EvaluationType?.ToString(),
                        IsPassed: m.IsPassed
                    );
                });

            // faltas do aluno desde 'since'
            var allAbs = await _absences.GetAllAsync();
            var myAbsences = allAbs
                .Where(a => a.StudentId == student.Id && a.Date >= sinceUtc)
                .Select(a =>
                {
                    var sid = a.SubjectId;
                    subjectNameById.TryGetValue(sid, out var sname);
                    return new FeedItem(
                        Type: "absence",
                        Id: a.Id,
                        SubjectId: sid,
                        Subject: sname,
                        Date: a.Date,
                        Value: null,
                        EvaluationType: null,
                        IsPassed: null
                    );
                });

            // junta, ordena por data desc, e limita (ex.: 200 itens)
            var items = myMarks
                .Concat(myAbsences)
                .OrderByDescending(i => i.Date)
                .Take(200)
                .ToList();

            return Ok(new
            {
                since = sinceUtc,
                count = items.Count,
                items
            });
        }
    }
}

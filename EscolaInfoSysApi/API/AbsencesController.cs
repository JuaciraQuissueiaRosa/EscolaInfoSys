using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Services;
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
    public class AbsencesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly ISubjectRepository _subjects;
        private readonly AbsenceStatsService _stats;

        public AbsencesController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            ISubjectRepository subjects,
            AbsenceStatsService stats)
        {
            _users = users;
            _students = students;
            _subjects = subjects;
            _stats = stats;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAbsences()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var stats = await _stats.GetAbsenceStatsAsync();

            var subjects = await _subjects.GetAllAsync();
            var nameById = subjects.ToDictionary(s => s.Id, s => s.Name);

            var myAbsences = stats.Absences
                .Where(a => a.StudentId == student.Id)
                .OrderByDescending(a => a.Date)
                .ToList();

            var items = myAbsences.Select(a =>
            {
                nameById.TryGetValue(a.SubjectId, out var sname);
                return new
                {
                    a.Id,
                    SubjectId = a.SubjectId,
                    Subject = sname,
                    a.Date,
                    a.Justified
                };
            }).ToList();

            var myExclusions = stats.Exclusions.Where(e => e.StudentId == student.Id).ToList();

            var perSubject = myAbsences
                .GroupBy(a => a.SubjectId)
                .Select(g =>
                {
                    var subjectId = g.Key;
                    var key = $"{student.Id}-{subjectId}";

                    var count = g.Count();
                    var pct = stats.Percentages.TryGetValue(key, out var v) ? v : 0.0;
                    var max = stats.MaxAbsences.TryGetValue(subjectId, out var m) ? m : 0;
                    var isExcluded = myExclusions.Any(e => e.SubjectId == subjectId && e.IsExcluded);

                    nameById.TryGetValue(subjectId, out var sname);
                    return new
                    {
                        SubjectId = subjectId,
                        Subject = sname,
                        Count = count,
                        Percentage = pct,
                        MaxAllowed = max,
                        Exceeded = count > max,
                        IsExcluded = isExcluded
                    };
                })
                .OrderBy(s => s.Subject)
                .ToList();

            var overall = new
            {
                AnyExcluded = perSubject.Any(s => s.IsExcluded),
                AnyExceeded = perSubject.Any(s => s.Exceeded)
            };

            return Ok(new { items, summary = new { perSubject, overall } });
        }
    }
}

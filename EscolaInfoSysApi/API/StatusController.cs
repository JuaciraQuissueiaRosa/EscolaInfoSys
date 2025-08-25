using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EscolaInfoSysApi.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly IMarkRepository _marks;
        private readonly ISubjectRepository _subjects;
        private readonly ISystemSettingsRepository _settings;
        private readonly AbsenceStatsService _absenceStats;

        public StatusController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            IMarkRepository marks,
            ISubjectRepository subjects,
            ISystemSettingsRepository settings,
            AbsenceStatsService absenceStats)
        {
            _users = users;
            _students = students;
            _marks = marks;
            _subjects = subjects;
            _settings = settings;
            _absenceStats = absenceStats;
        }

        public record SubjectStatusDto(
            int SubjectId,
            string Subject,
            double? Average,       // média do aluno na disciplina (0..20) se houver notas
            double? PassThreshold, // limiar para aprovação
            int Absences,          // total de faltas na disciplina
            int MaxAbsences,       // máximo permitido pela regra
            bool ExceededAbsences, // true => excedeu o limite
            string Status          // "Approved" | "Failed" | "Excluded"
        );

        // GET /api/status/per-subject
        [HttpGet("per-subject")]
        public async Task<ActionResult<IEnumerable<SubjectStatusDto>>> GetPerSubject()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            // 1) Threshold de aprovação (procura campo nos settings; fallback = 10)
            var settings = await _settings.GetSettingsAsync();
            var passThreshold = ResolvePassThreshold(settings) ?? 10.0;

            // 2) Marks do aluno
            var allMarks = await _marks.GetAllAsync();
            var myMarks = allMarks.Where(m => m.StudentId == student.Id && m.SubjectId.HasValue);

            var avgBySubject = myMarks
                .GroupBy(m => m.SubjectId!.Value)
                .ToDictionary(g => g.Key, g => (double)g.Average(m => m.Value));

            // 3) Absences (via serviço de estatísticas)
            var stats = await _absenceStats.GetAbsenceStatsAsync();

            var myAbsencesBySubject = stats.Absences
                .Where(a => a.StudentId == student.Id && a.SubjectId != 0)
                .GroupBy(a => a.SubjectId)
                .ToDictionary(g => g.Key, g => g.Count());

            // 4) Subjects para nomes
            var subs = await _subjects.GetAllAsync();
            var nameById = subs.ToDictionary(s => s.Id, s => s.Name);

            // 5) Monta a lista de disciplinas que aparecem em notas ou faltas
            var subjectIds = new HashSet<int>(avgBySubject.Keys);
            foreach (var sid in myAbsencesBySubject.Keys) subjectIds.Add(sid);

            var result = new List<SubjectStatusDto>();

            foreach (var subjectId in subjectIds.OrderBy(id => nameById.ContainsKey(id) ? nameById[id] : id.ToString()))
            {
                nameById.TryGetValue(subjectId, out var sname);
                var avg = avgBySubject.TryGetValue(subjectId, out var a) ? a : (double?)null;

                var absCount = myAbsencesBySubject.TryGetValue(subjectId, out var c) ? c : 0;
                var maxAbs = stats.MaxAbsences.TryGetValue(subjectId, out var mx) ? mx : 0;
                var exceeded = absCount > maxAbs;

                string status;
                if (exceeded) status = "Excluded";
                else if (avg.HasValue && avg.Value >= passThreshold) status = "Approved";
                else status = "Failed";

                result.Add(new SubjectStatusDto(
                    SubjectId: subjectId,
                    Subject: sname ?? string.Empty,
                    Average: avg is null ? null : Math.Round(avg.Value, 2),
                    PassThreshold: passThreshold,
                    Absences: absCount,
                    MaxAbsences: maxAbs,
                    ExceededAbsences: exceeded,
                    Status: status
                ));
            }

            return Ok(result);
        }

        // tenta encontrar um campo adequado nos teus SystemSettings (sem inventar nomes)
        private static double? ResolvePassThreshold(object settings)
        {
            if (settings is null) return null;
            var type = settings.GetType();

            // tenta propriedades comuns (case-insensitive)
            string[] candidates = new[]
            {
            "PassMark", "PassThreshold", "MinPassAverage", "MinAverageToPass",
            "ApprovalThreshold", "MinApprovalAverage", "PassingGrade"
        };

            foreach (var name in candidates)
            {
                var p = type.GetProperty(name);
                if (p is null) continue;

                var val = p.GetValue(settings);
                if (val is null) continue;

                // aceita int, double, decimal, float
                if (val is int i) return i;
                if (val is double d) return d;
                if (val is decimal m) return (double)m;
                if (val is float f) return f;
            }

            return null;
        }
    }
}

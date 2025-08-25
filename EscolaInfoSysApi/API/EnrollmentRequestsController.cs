using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EscolaInfoSysApi.API
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentRequestsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly ISubjectRepository _subjects;
        private readonly ApplicationDbContext _db;

        public EnrollmentRequestsController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            ISubjectRepository subjects,
            ApplicationDbContext db)
        {
            _users = users;
            _students = students;
            _subjects = subjects;
            _db = db;
        }

        public record CreateRequestDto(int SubjectId, string? Note);

        public record EnrollmentRequestDto(
            int Id,
            int StudentId,
            int SubjectId,
            string? Subject,
            string? Status,
            DateTime? CreatedAt
        );

        // GET /api/enrollment-requests/my
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<EnrollmentRequestDto>>> GetMy()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var (entityType, dbSetObj) = ResolveEnrollmentRequestSet();
            if (entityType is null || dbSetObj is null)
                return StatusCode(500, "Entidade EnrollmentRequest não encontrada no DbContext.");

            var studentIdProp = entityType.GetProperty("StudentId");
            var subjectIdProp = entityType.GetProperty("SubjectId");
            var idProp = entityType.GetProperty("Id");
            if (studentIdProp is null || subjectIdProp is null || idProp is null)
                return StatusCode(500, "Campos mínimos (Id, StudentId, SubjectId) não foram encontrados.");

            // materializa tudo e filtra em memória (simples; troca por consulta tipada se criares repo depois)
            var all = ((IEnumerable)dbSetObj).Cast<object>().ToList();
            var mine = all.Where(e => (int)studentIdProp.GetValue(e)! == student.Id).ToList();

            var subjects = await _subjects.GetAllAsync();
            var nameById = subjects.ToDictionary(s => s.Id, s => s.Name);

            // tenta ler Status/CreatedAt se existirem
            var statusProp = entityType.GetProperty("Status");
            var createdProp = entityType.GetProperty("CreatedAt");

            var result = mine
                .OrderByDescending(e =>
                {
                    var val = createdProp?.GetValue(e) as DateTime?;
                    return val ?? DateTime.MinValue;
                })
                .ThenByDescending(e => (int)idProp.GetValue(e)!)
                .Select(e =>
                {
                    var sid = (int)subjectIdProp.GetValue(e)!;
                    nameById.TryGetValue(sid, out var sname);
                    return new EnrollmentRequestDto(
                        Id: (int)idProp.GetValue(e)!,
                        StudentId: (int)studentIdProp.GetValue(e)!,
                        SubjectId: sid,
                        Subject: sname,
                        Status: statusProp?.GetValue(e)?.ToString(),
                        CreatedAt: createdProp?.GetValue(e) as DateTime?
                    );
                })
                .ToList();

            return Ok(result);
        }

        // POST /api/enrollment-requests
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRequestDto dto)
        {
            if (dto.SubjectId <= 0) return BadRequest("SubjectId inválido.");

            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var subject = await _subjects.GetByIdAsync(dto.SubjectId);
            if (subject is null) return NotFound("Disciplina não encontrada.");

            var (entityType, dbSetObj) = ResolveEnrollmentRequestSet();
            if (entityType is null || dbSetObj is null)
                return StatusCode(500, "Entidade EnrollmentRequest não encontrada no DbContext.");

            var studentIdProp = entityType.GetProperty("StudentId");
            var subjectIdProp = entityType.GetProperty("SubjectId");
            if (studentIdProp is null || subjectIdProp is null)
                return StatusCode(500, "Campos mínimos (StudentId, SubjectId) não foram encontrados.");

            var entity = Activator.CreateInstance(entityType)!;

            // obrigatórios
            studentIdProp.SetValue(entity, student.Id);
            subjectIdProp.SetValue(entity, dto.SubjectId);

            // opcionais se existirem
            entityType.GetProperty("CreatedAt")?.SetValue(entity, DateTime.UtcNow);
            entityType.GetProperty("Status")?.SetValue(entity, "Pending");
            (entityType.GetProperty("Note") ?? entityType.GetProperty("Comment"))?.SetValue(entity, dto.Note);

            _db.Add(entity);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMy), null);
        }

        // — helpers —

        private (Type? entityType, object? dbSetObj) ResolveEnrollmentRequestSet()
        {
            // tenta encontrar a CLR type mapeada chamada "EnrollmentRequest"
            var clr = _db.Model.GetEntityTypes()
                .Select(t => t.ClrType)
                .FirstOrDefault(t => t.Name == "EnrollmentRequest");

            if (clr is null) return (null, null);

            // encontra a propriedade DbSet<EnrollmentRequest> no DbContext
            var dbSetProp = _db.GetType().GetProperties()
                .FirstOrDefault(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                    p.PropertyType.GenericTypeArguments[0] == clr);

            var dbSetObj = dbSetProp?.GetValue(_db);
            return (clr, dbSetObj);
        }
    }
}

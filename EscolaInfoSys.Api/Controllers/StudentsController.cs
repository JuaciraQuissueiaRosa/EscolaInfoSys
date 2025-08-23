using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using EscolaInfoSys.Api.Models;

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/students")]
    [ApiController]
    //[Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IMarkRepository _markRepo;
        private readonly IAbsenceRepository _absenceRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(
            IStudentRepository studentRepo,
            IMarkRepository markRepo,
            IAbsenceRepository absenceRepo,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepo = studentRepo;
            _markRepo = markRepo;
            _absenceRepo = absenceRepo;
            _userManager = userManager;
        }

        // 🔹 GET: api/students/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetStudentProfile()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found");

            //  domínio correto do site onde as imagens realmente estão
            var baseUrl = "https://www.escolainfosys.somee.com";

            return Ok(new
            {
                student.Id,
                student.PupilNumber,
                student.CourseId,
                Course = student.Course?.Name,
                UserName = student.ApplicationUser?.UserName,
                Email = student.ApplicationUser?.Email,
                ProfilePhoto = string.IsNullOrEmpty(student.ProfilePhoto)
                    ? null
                    : $"{baseUrl}/uploads/{student.ProfilePhoto}"
            });
        }
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateStudentProfileAsync([FromForm] UpdateStudentProfileDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);

            if (student == null)
                return NotFound("Student not found");

            // Atualiza nome de usuário
            if (!string.IsNullOrEmpty(dto.UserName))
            {
                student.ApplicationUser.UserName = dto.UserName;
                student.ApplicationUser.NormalizedUserName = dto.UserName.ToUpper();

                var result = await _userManager.UpdateAsync(student.ApplicationUser);
                if (!result.Succeeded)
                    return BadRequest(new { errors = result.Errors });
            }

            // Atualiza foto de perfil, se enviada
            if (dto.ProfilePhoto != null && dto.ProfilePhoto.Length > 0)
            {
                if (dto.ProfilePhoto.Length > 2 * 1024 * 1024)
                    return BadRequest("Image file is too large (max 2MB).");

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(dto.ProfilePhoto.FileName).ToLowerInvariant();
                var mimeType = dto.ProfilePhoto.ContentType;

                if (!allowedExtensions.Contains(extension) || !mimeType.StartsWith("image/"))
                {
                    return BadRequest("Unsupported image format.");
                }

                // Gera nome único e salva
                var uniqueFileName = Guid.NewGuid() + extension;
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.ProfilePhoto.CopyToAsync(stream);

                student.ProfilePhoto = uniqueFileName;
            }

            await _studentRepo.UpdateAsync(student);

            return Ok(new { message = "Profile updated successfully." });
        }




        // 🔹 GET: api/students/marks
        [HttpGet("marks")]
        public async Task<IActionResult> GetMyMarks()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found");

            var allMarks = await _markRepo.GetAllAsync();
            var myMarks = allMarks
                .Where(m => m.StudentId == student.Id)
                .Select(m => new
                {
                    m.SubjectId,
                    Subject = m.Subject?.Name,
                    m.Value,
                    m.IsPassed,
                    m.Date
                }).ToList();

            return Ok(myMarks);
        }

        // 🔹 GET: api/students/absences
        [HttpGet("absences")]
        public async Task<IActionResult> GetMyAbsences()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found");

            var absences = await _absenceRepo.GetByStudentIdAsync(student.Id);
            var result = absences.Select(a => new
            {
                a.Id,
                a.Subject?.Name,
                a.Date,
                a.Justified
            }).ToList();

            return Ok(result);
        }

        [HttpGet("notifications")]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null)
                return NotFound("Student not found");

            var notifications = new List<object>();

            // Exemplo 1: Está excluído
            var isExcluded = await _studentRepo.IsStudentExcludedFromAnySubjectAsync(student.Id);

            if (isExcluded)
            {
                notifications.Add(new
                {
                    Type = "Exclusion",
                    Message = "You have been excluded from one or more subjects due to excessive absences."
                });
            }

            var marks = await _markRepo.GetAllAsync(); // ou algum método filtrado por aluno

            if (marks.Any())
            {
                var lastMark = marks.OrderByDescending(m => m.Date).First();
                notifications.Add(new
                {
                    Type = "Mark",
                    Message = $"New mark registered in {lastMark.Subject?.Name}: {lastMark.Value}."
                });
            }



            // Exemplo 3: Nova falta
            var absences = await _absenceRepo.GetByStudentIdAsync(student.Id);
            if (absences.Any())
            {
                var lastAbsence = absences.OrderByDescending(a => a.Date).First();
                notifications.Add(new
                {
                    Type = "Absence",
                    Message = $"Absence recorded in {lastAbsence.Subject?.Name} on {lastAbsence.Date.ToShortDateString()}."
                });
            }

            return Ok(notifications);
        }


    }
}


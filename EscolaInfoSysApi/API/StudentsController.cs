using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSysApi.API
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;
        private readonly IConfiguration _cfg;

        public StudentsController(
            UserManager<ApplicationUser> users,
            IStudentRepository students,
            IConfiguration cfg)
        {
            _users = users;
            _students = students;
            _cfg = cfg;
        }

        public record UpdateProfileRequest(string? FullName, string? ProfilePhoto);

        // Helper: extrai somente o nome do ficheiro de um path/URL
        private static string ExtractFileName(string? pathOrUrl)
        {
            if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;

            var s = pathOrUrl.Trim();
            if (Uri.TryCreate(s, UriKind.Absolute, out var uri))
                s = uri.AbsolutePath; // ex.: /uploads/abc.jpg

            var idx = s.LastIndexOf('/');
            if (idx >= 0 && idx < s.Length - 1)
                s = s[(idx + 1)..];

            return s;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileViewModel>> GetProfile()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            var roles = await _users.GetRolesAsync(user);

            // Mesma regra do site: se o Student tiver foto, usa-a; senão, usa a do Identity
            string? photo = !string.IsNullOrWhiteSpace(student?.ProfilePhoto)
                ? student!.ProfilePhoto
                : (!string.IsNullOrWhiteSpace(user.ProfilePhoto) ? user.ProfilePhoto : null);

            var vm = new ProfileViewModel
            {
                Email = user.Email ?? "",
                FullName = student?.FullName ?? user.Name ?? user.UserName ?? "",
                Role = roles.FirstOrDefault() ?? "User",
                ProfilePhoto = photo
            };

            return Ok(vm);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);
            if (student is null) return NotFound("Aluno não encontrado.");

            var changed = false;

            if (!string.IsNullOrWhiteSpace(req.FullName))
            {
                student.FullName = req.FullName;
                user.Name = req.FullName;
                changed = true;
            }

            if (!string.IsNullOrWhiteSpace(req.ProfilePhoto))
            {
                // grava o NOME do ficheiro em ambos (o site lê daí)
                var fileName = ExtractFileName(req.ProfilePhoto);
                if (!string.IsNullOrEmpty(fileName))
                {
                    user.ProfilePhoto = fileName;
                    student.ProfilePhoto = fileName;
                    changed = true;
                }
            }

            if (!changed) return NoContent();

            await _students.UpdateSelectedFieldsAsync(student);
            await _users.UpdateAsync(user);

            return NoContent();
        }

        // POST /api/students/profile/photo
        [HttpPost("profile/photo")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult> UploadProfilePhoto(IFormFile file, [FromServices] IWebHostEnvironment env)
        {
            if (file is null || file.Length == 0) return BadRequest("File required.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var ok = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!ok.Contains(ext)) return BadRequest("Invalid image type.");

            var fname = $"{Guid.NewGuid():N}{ext}";
            var root = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var dir = Path.Combine(root, "uploads");
            Directory.CreateDirectory(dir);

            var fullPath = Path.Combine(dir, fname);
            await using (var fs = System.IO.File.Create(fullPath))
                await file.CopyToAsync(fs);

            var me = await _users.GetUserAsync(User);
            if (me is null) return Unauthorized();

            // atualiza os dois lados para ficar igual ao site
            me.ProfilePhoto = fname;
            await _users.UpdateAsync(me);

            var student = await _students.GetByApplicationUserIdAsync(me.Id);
            if (student != null)
            {
                student.ProfilePhoto = fname;
                await _students.UpdateSelectedFieldsAsync(student);
            }

            var path = $"/uploads/{fname}";
            var webBase = _cfg["Web:BaseUrl"] ?? "https://www.escolainfosys.somee.com";
            var url = $"{webBase}{path}";

            // devolve path (o que o site usa) e url (para o app pré-visualizar)
            return Ok(new { path, url });
        }
    }


}


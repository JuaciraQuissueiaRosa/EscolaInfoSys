using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models.ViewModels;
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
    public class StudentsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly IStudentRepository _students;

        public StudentsController(UserManager<ApplicationUser> users, IStudentRepository students)
        {
            _users = users;
            _students = students;
        }

        public record UpdateProfileRequest(string? FullName, string? ProfilePhoto);

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileViewModel>> GetProfile()
        {
            var user = await _users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var student = await _students.GetByApplicationUserIdAsync(user.Id);

            var roles = await _users.GetRolesAsync(user);
            var vm = new ProfileViewModel
            {
                Email = user.Email ?? "",
                FullName = student?.FullName ?? user.Name ?? user.UserName ?? "",
                Role = roles.FirstOrDefault() ?? "User",
                ProfilePhoto = user.ProfilePhoto
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
                user.ProfilePhoto = req.ProfilePhoto;
                changed = true;
            }

            if (!changed) return NoContent();

            await _students.UpdateSelectedFieldsAsync(student);
            await _users.UpdateAsync(user);

            return NoContent();
        }

        [Authorize]
        [HttpPost("profile/photo")]
        public async Task<IActionResult> UpdateProfilePhoto([FromForm] IFormFile file,
                                                    [FromServices] IWebHostEnvironment env,
                                                    [FromServices] ApplicationDbContext db,
                                                    [FromServices] UserManager<ApplicationUser> users)
        {
            if (file is null || file.Length == 0) return BadRequest("No file.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowed.Contains(ext)) return BadRequest("Invalid file type.");

            // guarda em wwwroot/uploads
            var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
            var uploads = Path.Combine(webRoot, "uploads");
            Directory.CreateDirectory(uploads);

            var name = $"{Guid.NewGuid():N}{ext}";
            var full = Path.Combine(uploads, name);
            await using (var fs = System.IO.File.Create(full))
                await file.CopyToAsync(fs);

            var relPath = $"/uploads/{name}";
            var publicUrl = $"{Request.Scheme}://{Request.Host}{relPath}";

            // atualiza o utilizador atual
            var user = await users.GetUserAsync(User);
            if (user is null) return Unauthorized();

            user.ProfilePhoto = relPath; // <- campo já usado no teu perfil
            await db.SaveChangesAsync();

            // devolve caminho relativo (para gravar) + URL pública (para a app mostrar)
            return Ok(new { path = relPath, url = publicUrl });
        }
    }
}

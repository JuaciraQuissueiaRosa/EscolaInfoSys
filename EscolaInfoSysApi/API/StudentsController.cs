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
    }
}

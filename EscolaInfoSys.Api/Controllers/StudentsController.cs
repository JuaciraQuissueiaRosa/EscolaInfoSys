using EscolaInfoSys.Data.Repositories;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")] // Somente alunos autenticados
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(IStudentRepository studentRepo, UserManager<ApplicationUser> userManager)
        {
            _studentRepo = studentRepo;
            _userManager = userManager;
        }

        // GET: api/students/profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetStudentProfile()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);

            if (student == null)
                return NotFound("Student not found");

            return Ok(new
            {
                student.Id,
                student.PupilNumber,
                student.CourseId,
                student.Course?.Name,
                student.ApplicationUser?.UserName,
                student.ApplicationUser?.Email,
                student.ApplicationUser?.ProfilePhoto
            });
        }
    }
}


using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSysApi.API
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
 
    public class PublicController : ControllerBase
    {
        private readonly ICourseRepository _courses;
        private readonly ISubjectRepository _subjects;

        public PublicController(
            ICourseRepository courses,
            ISubjectRepository subjects)
        {
            _courses = courses;
            _subjects = subjects;
        }

        // GET /api/public/courses
        [HttpGet("courses")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCourses()
        {
            var list = await _courses.GetAllAsync();
            // Mantém só campos seguros/óbvios (Id, Name) para não depender de outras props
            var result = list
                .Select(c => new { c.Id, c.Name })
                .OrderBy(c => c.Name)
                .ToList();

            return Ok(result);
        }

        // GET /api/public/subjects
        // (opcional) ?courseId=123 para filtrar por curso
        [HttpGet("subjects")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubjects([FromQuery] int? courseId = null)
        {
            var list = await _subjects.GetAllAsync();

            if (courseId.HasValue)
                list = list.Where(s => s.CourseId == courseId.Value);

            var result = list
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.CourseId,
                    s.TotalLessons
                })
                .OrderBy(s => s.Name)
                .ToList();

            return Ok(result);
        }

        // GET /api/public/courses/{courseId}/subjects
        [HttpGet("courses/{courseId:int}/subjects")]
        [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubjectsByCourse(int courseId)
        {
            var course = await _courses.GetByIdAsync(courseId);
            if (course is null) return NotFound("Curso não encontrado.");

            var subs = await _subjects.GetAllAsync();
            var result = subs
                .Where(s => s.CourseId == courseId)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    s.CourseId,
                    s.TotalLessons
                })
                .OrderBy(s => s.Name)
                .ToList();

            return Ok(result);
        }
    }
}

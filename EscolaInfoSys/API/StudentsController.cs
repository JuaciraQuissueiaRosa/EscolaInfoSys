using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/students/by-formgroup?id=1
        [HttpGet("by-formgroup/{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByFormGroup(int id)
        {
            var students = await _context.Students
                .Include(s => s.FormGroup)
                .Include(s => s.Course)
                .Where(s => s.FormGroupId == id)
                .ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound($"No students found for FormGroup ID {id}");
            }

            return Ok(students);
        }


    }
}

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
        [HttpGet("by-formgroup")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByFormGroup(int id)
        {
            var formGroup = await _context.FormGroups
                .Include(f => f.Students)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formGroup == null)
            {
                return NotFound($"FormGroup with ID {id} not found.");
            }

            return Ok(formGroup.Students);
        }
    }
}

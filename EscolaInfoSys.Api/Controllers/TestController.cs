using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students
                .Include(s => s.ApplicationUser)
                .ToListAsync();

            return Ok(students);
        }
    }

}

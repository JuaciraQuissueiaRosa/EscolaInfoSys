using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
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
        private readonly IStudentRepository _studentRepo;

        public StudentsController(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
        }

        // GET: api/students/by-formgroup/1
        [HttpGet("by-formgroup/{id}")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByFormGroup(int id)
        {
            var students = await _studentRepo.GetWithFormGroupAsync();

            var filtered = students.Where(s => s.FormGroupId == id).ToList();

            if (filtered == null || filtered.Count == 0)
            {
                return NotFound($"No students found for FormGroup ID {id}");
            }

            return Ok(filtered);
        }
    }


}

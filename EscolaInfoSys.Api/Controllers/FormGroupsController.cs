using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/formgroups")]
    public class FormGroupsController : ControllerBase
    {
        private readonly IFormGroupRepository _formGroupRepo;
        private readonly IStudentRepository _studentRepo;

        public FormGroupsController(IFormGroupRepository formGroupRepo, IStudentRepository studentRepo)
        {
            _formGroupRepo = formGroupRepo;
            _studentRepo = studentRepo;
        }

        // GET: api/formgroups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FormGroup>>> GetAll()
        {
            var formGroups = await _formGroupRepo.GetAllAsync();
            return Ok(formGroups);
        }

        // GET: api/formgroups/{id}/students
        [HttpGet("{id}/students")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByFormGroup(int id)
        {
            var students = await _studentRepo.GetWithFormGroupAsync();
            var filtered = students.Where(s => s.FormGroupId == id).ToList();

            if (!filtered.Any())
                return NotFound("No students found for this FormGroup.");

            return Ok(filtered);
        }
    }
}

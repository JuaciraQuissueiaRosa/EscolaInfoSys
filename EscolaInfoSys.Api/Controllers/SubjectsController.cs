using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    [Authorize(Roles = "Administrator,StaffMember")]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectRepository _subjectRepo;

        public SubjectsController(ISubjectRepository subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subject>>> GetSubjects()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Subject>> GetSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<Subject>> CreateSubject(Subject subject)
        {
            await _subjectRepo.AddAsync(subject);
            return CreatedAtAction(nameof(GetSubject), new { id = subject.Id }, subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, Subject subject)
        {
            if (id != subject.Id)
                return BadRequest();

            await _subjectRepo.UpdateAsync(subject);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject == null)
                return NotFound();

            await _subjectRepo.DeleteAsync(subject);
            return NoContent();
        }
    }
}

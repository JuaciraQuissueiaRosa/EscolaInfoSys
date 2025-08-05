using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/public")]
    public class PublicController : ControllerBase
    {
        private readonly IFormGroupRepository _formGroupRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly ICourseRepository _courseRepo;

        public PublicController(IFormGroupRepository formGroupRepo, ISubjectRepository subjectRepo, ICourseRepository courseRepo)
        {
            _formGroupRepo = formGroupRepo;
            _subjectRepo = subjectRepo;
            _courseRepo = courseRepo;
        }

        // GET: api/public/formgroups
        [HttpGet("formgroups")]
        public async Task<IActionResult> GetFormGroups()
        {
            var groups = await _formGroupRepo.GetAllAsync();
            return Ok(groups.Select(g => new { g.Id, g.Name }));
        }

        // GET: api/public/subjects
        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return Ok(subjects.Select(s => new { s.Id, s.Name }));
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetCourses()
        {
            var courses = await _courseRepo.GetAllAsync();
            return Ok(courses.Select(c => new { c.Id, c.Name }));
        }

    }
}

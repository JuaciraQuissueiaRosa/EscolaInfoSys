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

        public PublicController(IFormGroupRepository formGroupRepo, ISubjectRepository subjectRepo)
        {
            _formGroupRepo = formGroupRepo;
            _subjectRepo = subjectRepo;
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
    }
}

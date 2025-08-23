using EscolaInfoSys.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/exclusions")]
    //[Authorize]
    public class ExclusionsController : ControllerBase
    {
        private readonly IStudentExclusionStatusService _exclusionService;

        public ExclusionsController(IStudentExclusionStatusService exclusionService)
        {
            _exclusionService = exclusionService;
        }

        // GET api/exclusions/check?studentId=1&subjectId=2
        [HttpGet("check")]
        public async Task<IActionResult> CheckExclusion(int studentId, int subjectId)
        {
            var isExcluded = await _exclusionService.IsStudentExcludedFromSubjectAsync(studentId, subjectId);
            return Ok(new { studentId, subjectId, isExcluded });
        }

        // GET api/exclusions/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllExclusions()
        {
            var result = await _exclusionService.GetAllExclusionsAsync();
            return Ok(result);
        }
    }

}

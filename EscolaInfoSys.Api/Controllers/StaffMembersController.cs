using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class StaffMembersController : ControllerBase
    {
        private readonly IStaffMemberRepository _staffRepo;

        public StaffMembersController(IStaffMemberRepository staffRepo)
        {
            _staffRepo = staffRepo;
        }

        // GET: api/staffmembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StaffMember>>> GetAll()
        {
            var staff = await _staffRepo.GetAllWithUserAsync();
            return Ok(staff);
        }

        // GET: api/staffmembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StaffMember>> Get(int id)
        {
            var staff = await _staffRepo.GetByIdWithUserAsync(id);
            if (staff == null) return NotFound();

            return Ok(staff);
        }

        // POST: api/staffmembers
        [HttpPost]
        public async Task<ActionResult<StaffMember>> Post([FromBody] StaffMember staff)
        {
            await _staffRepo.AddAsync(staff);
            return CreatedAtAction(nameof(Get), new { id = staff.Id }, staff);
        }

        // PUT: api/staffmembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StaffMember updated)
        {
            if (id != updated.Id) return BadRequest("ID mismatch");

            await _staffRepo.UpdateAsync(updated);
            return NoContent();
        }

        // DELETE: api/staffmembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff == null) return NotFound();

            await _staffRepo.DeleteAsync(staff);
            return NoContent();
        }
    }
}

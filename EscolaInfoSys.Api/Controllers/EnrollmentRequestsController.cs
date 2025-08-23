using EscolaInfoSys.Api.Models;
using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EscolaInfoSys.Api.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    //[Authorize]
    public class EnrollmentRequestsController : ControllerBase
    {
        private readonly IAlertRepository _alertRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public EnrollmentRequestsController(
            IAlertRepository alertRepo,
            IStudentRepository studentRepo,
            UserManager<ApplicationUser> userManager)
        {
            _alertRepo = alertRepo;
            _studentRepo = studentRepo;
            _userManager = userManager;
        }

        // POST: api/enrollments
        [HttpPost]
      
        public async Task<IActionResult> Post([FromBody] CreateEnrollmentRequestDto dto)
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null) return Unauthorized();

            var request = new Alert
            {
                Title = "Enrollment Request",
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                IsResolved = false,
                StaffId = student.Id
            };

            await _alertRepo.AddAsync(request);
            return Ok(new { message = "Enrollment request submitted." });
        }


        // GET: api/enrollments/mine
        [HttpGet("mine")]
       
        public async Task<IActionResult> GetMine()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null) return Unauthorized();

            var requests = await _alertRepo.GetByStaffIdAsync(student.Id); // StaffId = StudentId aqui
            var enrollmentRequests = requests.Where(r => r.Title == "Enrollment Request");
            return Ok(enrollmentRequests);
        }

        // GET: api/enrollments
        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
            var alerts = await _alertRepo.GetAllWithStaffAsync();
            var requests = alerts.Where(a => a.Title == "Enrollment Request");
            return Ok(requests);
        }

        // PUT: api/enrollments/{id}/approve
        [HttpPut("{id}/approve")]
        
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _alertRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.AdminResponse = "Approved";
            request.IsResolved = true;
            await _alertRepo.UpdateAsync(request);

            return Ok(new { message = "Request approved." });
        }

        // PUT: api/enrollments/{id}/reject
        [HttpPut("{id}/reject")]
        
        public async Task<IActionResult> Reject(int id)
        {
            var request = await _alertRepo.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.AdminResponse = "Rejected";
            request.IsResolved = true;
            await _alertRepo.UpdateAsync(request);

            return Ok(new { message = "Request rejected." });
        }
    }
}


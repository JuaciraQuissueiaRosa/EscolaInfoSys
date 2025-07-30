using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Services;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IMarkRepository _markRepo;
        private readonly IAbsenceRepository _absenceRepo;
        ISystemSettingsRepository _settingsRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;

        public StudentPortalController(
            IStudentRepository studentRepo,
            IMarkRepository markRepo,
            IAbsenceRepository absenceRepo,
            UserManager<ApplicationUser> userManager,
               ISystemSettingsRepository settingsRepo,
               IStudentService studentService)
        {
            _studentRepo = studentRepo;
            _markRepo = markRepo;
            _absenceRepo = absenceRepo;
            _userManager = userManager;
            _settingsRepo = settingsRepo;
            _studentService = studentService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var student = await _studentRepo.GetByApplicationUserIdAsync(user.Id);
                if (student == null) return NotFound();

                return View(student);
            }
            catch (Exception ex)
            {
                return Content("Erro: " + ex.Message);
            }
        }

        public async Task<IActionResult> MyMarks()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);

            if (student == null) return NotFound();

            var marks = await _markRepo.GetAllAsync();
            var myMarks = marks.Where(m => m.StudentId == student.Id).ToList();

            return View(myMarks); 
        }

        public async Task<IActionResult> MyAbsences()
        {
            var userId = _userManager.GetUserId(User);

            var (absences, isExcluded) = await _studentService.GetStudentAbsencesAndExclusionAsync(userId);


            return View(absences);
        }





    }
}

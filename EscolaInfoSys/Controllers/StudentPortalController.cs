using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        public StudentPortalController(
            IStudentRepository studentRepo,
            IMarkRepository markRepo,
            IAbsenceRepository absenceRepo,
            UserManager<ApplicationUser> userManager,
               ISystemSettingsRepository settingsRepo)
        {
            _studentRepo = studentRepo;
            _markRepo = markRepo;
            _absenceRepo = absenceRepo;
            _userManager = userManager;
            _settingsRepo = settingsRepo;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);

            if (student == null) return NotFound();

            return View(student); 
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
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);

            if (student == null) return NotFound();

            var allAbsences = await _absenceRepo.GetAllAsync();
            var myAbsences = allAbsences.Where(a => a.StudentId == student.Id).ToList();

            var settings = await _settingsRepo.GetSettingsAsync();
            double maxAllowedAbsences = settings?.MaxAbsencePercentage ?? 30.0; // interpretado como "30 faltas"

            int totalAbsences = myAbsences.Count;
            double progress = maxAllowedAbsences == 0 ? 0 : (100.0 * totalAbsences / maxAllowedAbsences);
            progress = Math.Min(progress, 150); // evitar quebrar layout se for muito alto

            ViewBag.AbsencePercentage = Math.Round(progress, 1);
            ViewBag.AbsenceLimit = maxAllowedAbsences;

            return View(myAbsences);
        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using EscolaInfoSys.Services;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EscolaInfoSys.Hubs;
using Microsoft.AspNetCore.SignalR;
using EscolaInfoSys.Models.ViewModels;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "StaffMember")]
    public class AbsencesController : Controller
    {
        private readonly IAbsenceRepository _absenceRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly AbsenceCheckerService _absenceChecker;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IStudentExclusionRepository _exclusionRepo;
        private readonly AbsenceStatsService _absenceStats;


        public AbsencesController(
            IAbsenceRepository absenceRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            AbsenceCheckerService absenceChecker,
            IHubContext<NotificationHub> hubContext,
             IStudentExclusionRepository exclusionRepo,
             AbsenceStatsService absenceStats)
        {
            _absenceRepo = absenceRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _absenceChecker = absenceChecker;
            _hubContext = hubContext;
            _exclusionRepo = exclusionRepo;
            _absenceStats = absenceStats;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _absenceStats.GetAbsenceStatsAsync();

            ViewBag.Exclusions = stats.Exclusions;
            ViewBag.Percentages = stats.Percentages;
            ViewBag.MaxAbsences = stats.MaxAbsences;

            return View(stats.Absences);
        }





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var absence = await _absenceRepo.GetByIdWithStudentAndSubjectAsync(id.Value);
            if (absence == null) return NotFound();

            return View(absence);
        }


        public async Task<IActionResult> Create()
        {
            var students = await _studentRepo.GetAllAsync();
            ViewData["StudentId"] = new SelectList(students, "Id", "FullName");

            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name");
            return View(); 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Absence absence)
        {
            if (ModelState.IsValid)
            {
                await _absenceRepo.AddAsync(absence);

                // Só chama o checker se ambos os campos estiverem preenchidos
                if (absence.StudentId > 0 && absence.SubjectId > 0)
                {
                    await _absenceChecker.CheckExclusionAsync(absence.StudentId, absence.SubjectId);
                }

                TempData["Message"] = "Absence successfully registered!";
                TempData["MessageType"] = "info";
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "FullName", absence.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", absence.SubjectId);
            return View(absence);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _absenceRepo.GetAbsenceEditViewModelAsync(id);
            if (model == null) return NotFound();

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", model.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", model.SubjectId);

            return View(model);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AbsenceEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _absenceRepo.UpdateFromViewModelAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", model.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", model.SubjectId);
            return View(model);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var absence = await _absenceRepo.GetByIdWithDetailsAsync(id.Value); 
            if (absence == null) return NotFound();

            return View(absence);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var absence = await _absenceRepo.GetByIdAsync(id);
            if (absence != null)
            {
                await _absenceRepo.DeleteAsync(absence);
            }
            return RedirectToAction(nameof(Index));
        }
    }


}

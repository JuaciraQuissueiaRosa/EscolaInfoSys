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

        public AbsencesController(
            IAbsenceRepository absenceRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            AbsenceCheckerService absenceChecker,
            IHubContext<NotificationHub> hubContext)
        {
            _absenceRepo = absenceRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _absenceChecker = absenceChecker;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var absences = await _absenceRepo.GetAllAsync();
            return View(absences);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var absence = await _absenceRepo.GetByIdAsync(id.Value);
            if (absence == null) return NotFound();

            return View(absence);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email");
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,StudentId,SubjectId,Justified")] Absence absence)
        {
            if (ModelState.IsValid)
            {
                await _absenceRepo.AddAsync(absence);
                await _absenceChecker.CheckExclusionAsync(absence.StudentId, absence.SubjectId);

               
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", "Absence successfully registered!", "info");

                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", absence.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", absence.SubjectId);
            return View(absence);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var absence = await _absenceRepo.GetByIdAsync(id.Value);
            if (absence == null) return NotFound();

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", absence.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", absence.SubjectId);
            return View(absence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,StudentId,SubjectId,Justified")] Absence absence)
        {
            if (id != absence.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _absenceRepo.UpdateAsync(absence);
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", absence.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", absence.SubjectId);
            return View(absence);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var absence = await _absenceRepo.GetByIdAsync(id.Value);
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

using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator,StaffMember")]
    public class AbsencesController : Controller
    {
        private readonly IAbsenceRepository _repository;
        private readonly ApplicationDbContext _context;

        public AbsencesController(IAbsenceRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var absences = await _repository.GetAllAsync();
            return View(absences);
        }

        public async Task<IActionResult> Details(int id)
        {
            var absence = await _repository.GetByIdAsync(id);
            if (absence == null) return NotFound();
            return View(absence);
        }

        public IActionResult Create()
        {
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName");
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Absence absence)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(absence);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", absence.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", absence.SubjectId);
            return View(absence);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var absence = await _repository.GetByIdAsync(id);
            if (absence == null) return NotFound();

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", absence.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", absence.SubjectId);
            return View(absence);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Absence absence)
        {
            if (id != absence.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(absence);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", absence.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", absence.SubjectId);
            return View(absence);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var absence = await _repository.GetByIdAsync(id);
            if (absence == null) return NotFound();
            return View(absence);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var absence = await _repository.GetByIdAsync(id);
            if (absence == null) return NotFound();

            await _repository.DeleteAsync(absence);
            return RedirectToAction(nameof(Index));
        }
    }
}

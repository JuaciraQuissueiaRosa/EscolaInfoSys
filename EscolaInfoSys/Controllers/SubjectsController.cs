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
    public class SubjectsController : Controller
    {
        private readonly ISubjectRepository _repository;
        private readonly ApplicationDbContext _context;

        public SubjectsController(ISubjectRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _repository.GetAllAsync();
            return View(subjects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        public IActionResult Create()
        {
            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(subject);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(subject);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CourseId = new SelectList(_context.Courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _repository.GetByIdAsync(id);
            if (subject == null) return NotFound();

            await _repository.DeleteAsync(subject);
            return RedirectToAction(nameof(Index));
        }
    }
}

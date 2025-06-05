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
    public class MarksController : Controller
    {
        private readonly IMarkRepository _repository;
        private readonly ApplicationDbContext _context;

        public MarksController(IMarkRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var marks = await _repository.GetAllAsync();
            return View(marks);
        }

        public async Task<IActionResult> Details(int id)
        {
            var mark = await _repository.GetByIdAsync(id);
            if (mark == null) return NotFound();
            return View(mark);
        }

        public IActionResult Create()
        {
            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName");
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mark mark)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(mark);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var mark = await _repository.GetByIdAsync(id);
            if (mark == null) return NotFound();

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mark mark)
        {
            if (id != mark.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(mark);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewBag.SubjectId = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var mark = await _repository.GetByIdAsync(id);
            if (mark == null) return NotFound();
            return View(mark);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _repository.GetByIdAsync(id);
            if (mark == null) return NotFound();

            await _repository.DeleteAsync(mark);
            return RedirectToAction(nameof(Index));
        }
    }
}

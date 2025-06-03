using EscolaInfoSys.Data;
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
        private readonly ApplicationDbContext _context;

        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var marks = _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember);
            return View(await marks.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mark mark)
        {
            if (ModelState.IsValid)
            {
                mark.Date = DateTime.Now;
                _context.Add(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "FullName", mark.StaffMemberId);
            return View(mark);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mark == null) return NotFound();

            return View(mark);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null) return NotFound();

            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "FullName", mark.StaffMemberId);
            return View(mark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mark mark)
        {
            if (id != mark.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Marks.Any(m => m.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "FullName", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "FullName", mark.StaffMemberId);
            return View(mark);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mark == null) return NotFound();

            return View(mark);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            _context.Marks.Remove(mark);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

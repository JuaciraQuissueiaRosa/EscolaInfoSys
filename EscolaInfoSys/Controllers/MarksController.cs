using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;

namespace EscolaInfoSys.Controllers
{
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Marks
        public async Task<IActionResult> Index()
        {
            var marks = await _context.Marks
       .Include(m => m.StaffMember)
       .Include(m => m.Student)
       .Include(m => m.Subject)
       .ToListAsync();

            var studentIds = marks.Select(m => m.StudentId).Distinct();
            var subjectIds = marks.Select(m => m.SubjectId).Distinct();

            var exclusions = await _context.StudentExclusions
                .Where(e => studentIds.Contains(e.StudentId) && subjectIds.Contains(e.SubjectId))
                .ToListAsync();

            ViewBag.Exclusions = exclusions;

            return View(marks);
        }

        // GET: Marks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks
                .Include(m => m.StaffMember)
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // GET: Marks/Create
        public IActionResult Create()
        {
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "Email");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Email");
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id");
            return View();
        }

        // POST: Marks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,Date,StudentId,SubjectId,EvaluationType,StaffMemberId")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "Email", mark.StaffMemberId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", mark.SubjectId);
            return View(mark);
        }

        // GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null)
            {
                return NotFound();
            }
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "Email", mark.StaffMemberId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", mark.SubjectId);
            return View(mark);
        }

        // POST: Marks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value,Date,StudentId,SubjectId,EvaluationType,StaffMemberId")] Mark mark)
        {
            if (id != mark.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(mark.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StaffMemberId"] = new SelectList(_context.StaffMembers, "Id", "Email", mark.StaffMemberId);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Id", mark.SubjectId);
            return View(mark);
        }

        // GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Marks
                .Include(m => m.StaffMember)
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            if (mark != null)
            {
                _context.Marks.Remove(mark);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Marks.Any(e => e.Id == id);
        }
    }
}

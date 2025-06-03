using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator,StaffMember")]
    public class FormGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FormGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.FormGroups.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.FormGroups
                .Include(f => f.Students)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (group == null) return NotFound();

            return View(group);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FormGroup formGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(formGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(formGroup);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.FormGroups.FindAsync(id);
            if (group == null) return NotFound();

            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormGroup formGroup)
        {
            if (id != formGroup.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(formGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.FormGroups.Any(e => e.Id == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(formGroup);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var group = await _context.FormGroups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null) return NotFound();

            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.FormGroups.FindAsync(id);
            _context.FormGroups.Remove(group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

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
    public class StaffMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StaffMembers
        public async Task<IActionResult> Index()
        {
            return View(await _context.StaffMembers.ToListAsync());
        }

        // GET: StaffMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }

            return View(staffMember);
        }

        // GET: StaffMembers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StaffMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Email")] StaffMember staffMember)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staffMember);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(staffMember);
        }

        // GET: StaffMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffMember = await _context.StaffMembers.FindAsync(id);
            if (staffMember == null)
            {
                return NotFound();
            }
            return View(staffMember);
        }

        // POST: StaffMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email")] StaffMember staffMember)
        {
            if (id != staffMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffMemberExists(staffMember.Id))
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
            return View(staffMember);
        }

        // GET: StaffMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }

            return View(staffMember);
        }

        // POST: StaffMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffMember = await _context.StaffMembers.FindAsync(id);
            if (staffMember != null)
            {
                _context.StaffMembers.Remove(staffMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffMemberExists(int id)
        {
            return _context.StaffMembers.Any(e => e.Id == id);
        }
    }
}

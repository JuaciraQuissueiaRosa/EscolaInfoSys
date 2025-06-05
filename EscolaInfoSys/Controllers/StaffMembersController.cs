using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator,StaffMember")]
    public class StaffMembersController : Controller
    {
        private readonly IStaffMemberRepository _repository;

        public StaffMembersController(IStaffMemberRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var staff = await _repository.GetAllAsync();
            return View(staff);
        }

        public async Task<IActionResult> Details(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffMember staff)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(staff);
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffMember staff)
        {
            if (id != staff.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(staff);
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null) return NotFound();
            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _repository.GetByIdAsync(id);
            if (staff == null) return NotFound();

            await _repository.DeleteAsync(staff);
            return RedirectToAction(nameof(Index));
        }
    }
}

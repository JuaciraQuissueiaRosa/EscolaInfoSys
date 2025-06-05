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
    public class FormGroupsController : Controller
    {
        private readonly IFormGroupRepository _repository;

        public FormGroupsController(IFormGroupRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var formGroups = await _repository.GetAllAsync();
            return View(formGroups);
        }

        public async Task<IActionResult> Details(int id)
        {
            var group = await _repository.GetByIdAsync(id);
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
                await _repository.AddAsync(formGroup);
                return RedirectToAction(nameof(Index));
            }
            return View(formGroup);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var group = await _repository.GetByIdAsync(id);
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
                await _repository.UpdateAsync(formGroup);
                return RedirectToAction(nameof(Index));
            }
            return View(formGroup);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null) return NotFound();

            await _repository.DeleteAsync(group);
            return RedirectToAction(nameof(Index));
        }
    }
}

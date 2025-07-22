using Microsoft.AspNetCore.Mvc;
using EscolaInfoSys.Models;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using EscolaInfoSys.Data.Repositories;

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

        // GET: FormGroups
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var formGroups = await _repository.GetAllWithStudentsAndSubjectsAsync();
            return View(formGroups);
        }


        // GET: FormGroups/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var formGroup = await _repository.GetByIdWithStudentsAndSubjectsAsync(id);
            if (formGroup == null) return NotFound();
            return View(formGroup);
        }

        // GET: FormGroups/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FormGroups/Create
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

        // GET: FormGroups/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: FormGroups/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FormGroup formGroup)
        {
            if (id != formGroup.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(formGroup);
                return RedirectToAction(nameof(Index));
            }

            return View(formGroup);
        }

        // GET: FormGroups/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null)
                return NotFound();

            return View(group);
        }

        // POST: FormGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _repository.GetByIdAsync(id);
            if (group == null)
                return NotFound();

            await _repository.DeleteAsync(group);
            return RedirectToAction(nameof(Index));
        }
    }
}

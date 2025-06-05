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
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _repository;
        private readonly ApplicationDbContext _context;

        public StudentsController(IStudentRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context; // usado para carregar o ViewBag.FormGroupId
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _repository.GetAllAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewBag.FormGroupId = new SelectList(_context.FormGroups, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine("wwwroot/images", fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);
                    student.ProfilePhoto = "/images/" + fileName;
                }

                await _repository.AddAsync(student);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FormGroupId = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            ViewBag.FormGroupId = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(student);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.FormGroupId = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return NotFound();

            await _repository.DeleteAsync(student);
            return RedirectToAction(nameof(Index));
        }
    }
}

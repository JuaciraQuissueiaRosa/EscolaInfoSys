using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using EscolaInfoSys.Data.Repositories.Interfaces;

namespace EscolaInfoSys.Controllers
{
    public class SubjectsController : Controller
    {
        private readonly ISubjectRepository _subjectRepo;
        private readonly ICourseRepository _courseRepo;

        public SubjectsController(ISubjectRepository subjectRepo, ICourseRepository courseRepo)
        {
            _subjectRepo = subjectRepo;
            _courseRepo = courseRepo;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepo.GetAllAsync();
            return View(subjects);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectRepo.GetByIdAsync(id.Value);
            if (subject == null) return NotFound();

            return View(subject);
        }

        public async Task<IActionResult> Create()
        {
            var courses = await _courseRepo.GetAllAsync();
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CourseId")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                await _subjectRepo.AddAsync(subject);
                return RedirectToAction(nameof(Index));
            }

            var courses = await _courseRepo.GetAllAsync();
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectRepo.GetByIdAsync(id.Value);
            if (subject == null) return NotFound();

            var courses = await _courseRepo.GetAllAsync();
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CourseId")] Subject subject)
        {
            if (id != subject.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _subjectRepo.UpdateAsync(subject);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _subjectRepo.ExistsAsync(subject.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var courses = await _courseRepo.GetAllAsync();
            ViewData["CourseId"] = new SelectList(courses, "Id", "Name", subject.CourseId);
            return View(subject);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var subject = await _subjectRepo.GetByIdAsync(id.Value);
            if (subject == null) return NotFound();

            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subject = await _subjectRepo.GetByIdAsync(id);
            if (subject != null)
            {
                await _subjectRepo.DeleteAsync(subject);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}

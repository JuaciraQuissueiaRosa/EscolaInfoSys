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
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        // GET: Courses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create() => View();

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                await _courseRepository.AddAsync(course);
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _courseRepository.UpdateAsync(course);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    if (!await _courseRepository.ExistsAsync(course.Id))
                        return NotFound();

                    throw;
                }
            }

            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course != null)
            {
                await _courseRepository.DeleteAsync(course);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

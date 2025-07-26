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
    [Authorize(Roles = "Administrator")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IFormGroupRepository _formGroupRepository;

        public CoursesController(ICourseRepository courseRepository, IFormGroupRepository formGroupRepository)
        {
            _courseRepository = courseRepository;
            _formGroupRepository = formGroupRepository;
        }

        // GET: Courses
        [AllowAnonymous]

        // GET: Courses/Index
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllWithSubjectsAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var course = await _courseRepository.GetByIdWithSubjectsAsync(id.Value);
            if (course == null) return NotFound();

            return View(course);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            var allFormGroups = await _formGroupRepository.GetAllWithStudentsAndSubjectsAsync();
            ViewBag.FormGroups = new MultiSelectList(allFormGroups, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Course course, int[] SelectedFormGroupIds)
        {
            if (ModelState.IsValid)
            {
                if (SelectedFormGroupIds != null && SelectedFormGroupIds.Length > 0)
                {
                    var formGroups = (await _formGroupRepository.GetAllWithStudentsAndSubjectsAsync())
                        .Where(fg => SelectedFormGroupIds.Contains(fg.Id))
                        .ToList();
                    course.FormGroups = formGroups;
                }

                await _courseRepository.AddAsync(course);
                return RedirectToAction(nameof(Index));
            }

            var allFormGroups = await _formGroupRepository.GetAllWithStudentsAndSubjectsAsync();
            ViewBag.FormGroups = new MultiSelectList(allFormGroups, "Id", "Name", SelectedFormGroupIds);
            return View(course);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var course = await _courseRepository.GetByIdWithSubjectsAsync(id.Value);
            if (course == null) return NotFound();

            var allFormGroups = await _formGroupRepository.GetAllWithStudentsAndSubjectsAsync();
            var selectedIds = await _courseRepository.GetFormGroupIdsByCourseIdAsync(id.Value);

            ViewBag.FormGroups = new MultiSelectList(allFormGroups, "Id", "Name", selectedIds);

            return View(course);
        }


        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Course course, int[] SelectedFormGroupIds)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await _courseRepository.GetByIdWithSubjectsAsync(id);
                    if (existingCourse == null) return NotFound();

                    existingCourse.Name = course.Name;
                    existingCourse.Description = course.Description;

                    // Atualiza FormGroups usando método do repositório
                    await _courseRepository.UpdateCourseFormGroupsAsync(id, SelectedFormGroupIds ?? Enumerable.Empty<int>());

                    await _courseRepository.UpdateAsync(existingCourse);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    if (!await _courseRepository.ExistsAsync(course.Id)) return NotFound();
                    throw;
                }
            }

            var allFormGroups = await _formGroupRepository.GetAllWithStudentsAndSubjectsAsync();
            ViewBag.FormGroups = new MultiSelectList(allFormGroups, "Id", "Name", SelectedFormGroupIds);
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

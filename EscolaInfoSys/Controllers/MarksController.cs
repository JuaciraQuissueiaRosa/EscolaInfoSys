using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "StaffMember")]
    public class MarksController : Controller
    {
        private readonly IMarkRepository _markRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IStaffMemberRepository _staffRepo;
        private readonly IStudentExclusionRepository _exclusionRepo;

        public MarksController(
            IMarkRepository markRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            IStaffMemberRepository staffRepo,
            IStudentExclusionRepository exclusionRepo)
        {
            _markRepo = markRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _staffRepo = staffRepo;
            _exclusionRepo = exclusionRepo;
        }

        public async Task<IActionResult> Index()
        {
            var (excludedStudentIds, excludedSubjectIds) = await _exclusionRepo.GetExcludedStudentAndSubjectIdsAsync();

            var marks = await _markRepo.GetValidMarksAsync(excludedStudentIds, excludedSubjectIds);

            ViewBag.Exclusions = await _exclusionRepo.GetAllAsync(); //disponível para exibição na View
           

            return View(marks);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _markRepo.GetByIdAsync(id.Value);
            if (mark == null) return NotFound();

            return View(mark);
        }


        public async Task<IActionResult> Create()
        {
            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email");
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name");
            ViewData["StaffMemberId"] = new SelectList(await _staffRepo.GetAllAsync(), "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,Date,StudentId,SubjectId,EvaluationType,StaffMemberId")] Mark mark)
        {
            if (ModelState.IsValid)
            {
                await _markRepo.AddAsync(mark);
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(await _staffRepo.GetAllAsync(), "Id", "Email", mark.StaffMemberId);
            return View(mark);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _markRepo.GetByIdAsync(id.Value);
            if (mark == null) return NotFound();

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(await _staffRepo.GetAllAsync(), "Id", "Email", mark.StaffMemberId);
            return View(mark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value,Date,StudentId,SubjectId,EvaluationType,StaffMemberId")] Mark mark)
        {
            if (id != mark.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _markRepo.UpdateAsync(mark);
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentId"] = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);
            ViewData["StaffMemberId"] = new SelectList(await _staffRepo.GetAllAsync(), "Id", "Email", mark.StaffMemberId);
            return View(mark);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _markRepo.GetByIdAsync(id.Value);
            if (mark == null) return NotFound();

            return View(mark);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _markRepo.GetByIdAsync(id);
            if (mark != null)
            {
                await _markRepo.DeleteAsync(mark);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MarkExists(int id)
        {
            return await _markRepo.ExistsAsync(id);
        }
    }

}

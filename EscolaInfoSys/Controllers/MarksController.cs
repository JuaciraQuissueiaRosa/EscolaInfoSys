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
using Microsoft.AspNetCore.Identity;
using EscolaInfoSys.Services;
using EscolaInfoSys.Helper;

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
        private readonly IAccountService _accountService;

        public MarksController(
            IMarkRepository markRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            IStaffMemberRepository staffRepo,
            IStudentExclusionRepository exclusionRepo,
            IAccountService accountService
            )
        {
            _markRepo = markRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _staffRepo = staffRepo;
            _exclusionRepo = exclusionRepo;
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            var staff = await _staffRepo.GetByApplicationUserIdAsync(user.Id);

            if (staff == null)
                return Unauthorized();

            var (excludedStudentIds, excludedSubjectIds) = await _exclusionRepo.GetExcludedStudentAndSubjectIdsAsync();

            var allMarks = await _markRepo.GetValidMarksAsync(excludedStudentIds, excludedSubjectIds);

            var marks = allMarks.Where(m => m.StaffMemberId == staff.Id).ToList();

            ViewBag.Exclusions = await _exclusionRepo.GetAllAsync();

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
            ViewBag.StudentId = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email");
            ViewBag.SubjectId = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name");

            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized();

            var staff = await _staffRepo.GetByApplicationUserIdAsync(user.Id);
            if (staff == null)
                return Unauthorized();

            ViewBag.EvaluationTypeList = EnumHelper.GetSelectList<EvaluationType>();

            // Passa o StaffMemberId e o email para a view
            ViewBag.StaffMemberId = staff.Id;
            ViewBag.StaffEmail = staff.ApplicationUser.Email; // email para exibir

            var mark = new Mark { StaffMemberId = staff.Id };
            return View(mark);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mark mark)
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null)
                return Unauthorized();

            var staff = await _staffRepo.GetByApplicationUserIdAsync(user.Id);
            if (staff == null)
                return Unauthorized();

            // Garante que só o staff logado será atribuído
            mark.StaffMemberId = staff.Id;

            // Remove StaffMemberId do ModelState para evitar erros na validação
            ModelState.Remove(nameof(mark.StaffMemberId));

            if (ModelState.IsValid)
            {
                await _markRepo.AddAsync(mark);

                await _markRepo.UpdateStudentSubjectPassStatusAsync(mark.StudentId.Value, mark.SubjectId.Value);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewBag.SubjectId = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);
            ViewBag.StaffMemberId = staff.Id;
            ViewBag.StaffEmail = staff.ApplicationUser.Email;
            ViewBag.EvaluationTypeList = EnumHelper.GetSelectList<EvaluationType>();

            return View(mark);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _markRepo.GetByIdAsync(id.Value);
            if (mark == null) return NotFound();

            ViewBag.StudentId = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewBag.SubjectId = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);

            ViewBag.EvaluationTypeList = EnumHelper.GetSelectList<EvaluationType>();

            ViewBag.StaffMemberId = mark.StaffMemberId;

            return View(mark);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mark mark)
        {
            if (id != mark.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _markRepo.UpdateAsync(mark);
                await _markRepo.UpdateStudentSubjectPassStatusAsync(mark.StudentId.Value, mark.SubjectId.Value);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.StudentId = new SelectList(await _studentRepo.GetAllAsync(), "Id", "Email", mark.StudentId);
            ViewBag.SubjectId = new SelectList(await _subjectRepo.GetAllAsync(), "Id", "Name", mark.SubjectId);

            ViewBag.EvaluationTypeList = EnumHelper.GetSelectList<EvaluationType>();

            ViewBag.StaffMemberId = mark.StaffMemberId;

            return View(mark);
        }

        public async Task<IActionResult> Averages()
        {
            var user = await _accountService.GetCurrentUserAsync(User);
            if (user == null) return Unauthorized();

            var staff = await _staffRepo.GetByApplicationUserIdAsync(user.Id);
            if (staff == null) return Unauthorized();

            var averages = await _markRepo.GetStudentSubjectAveragesByStaffAsync(staff.Id);

            return View(averages);
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

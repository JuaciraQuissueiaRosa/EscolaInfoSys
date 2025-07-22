using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using EscolaInfoSys.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator,StaffMember")]
    public class StudentsController : Controller
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IFormGroupRepository _formGroupRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public StudentsController(
            IStudentRepository studentRepo,
            IFormGroupRepository formGroupRepo,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _studentRepo = studentRepo;
            _formGroupRepo = formGroupRepo;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentRepo.GetWithFormGroupAsync(); 
            return View(students);
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentRepo.GetFullByIdAsync(id.Value);
            if (student == null) return NotFound();

            var exclusions = await _studentRepo.GetExclusionsAsync(student.Id);
            ViewBag.Exclusions = exclusions;

            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            var formGroups = await _formGroupRepo.GetAllAsync();
            ViewData["FormGroupId"] = new SelectList(formGroups, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile ProfilePhotoFile, IFormFile DocumentPhotoFile)
        {
            if (ModelState.IsValid)
            {
                // Upload de fotos
                if (ProfilePhotoFile != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(ProfilePhotoFile.FileName);
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await ProfilePhotoFile.CopyToAsync(stream);
                    student.ProfilePhoto = fileName;
                }

                if (DocumentPhotoFile != null)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(DocumentPhotoFile.FileName);
                    var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    using var stream = new FileStream(path, FileMode.Create);
                    await DocumentPhotoFile.CopyToAsync(stream);
                    student.DocumentPhoto = fileName;
                }

                var user = new ApplicationUser
                {
                    UserName = student.Email,
                    Email = student.Email,
                    EmailConfirmed = false
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);

                    var formGroups = await _formGroupRepo.GetAllAsync();
                    ViewData["FormGroupId"] = new SelectList(formGroups, "Id", "Name", student.FormGroupId);
                    return View(student);
                }

                await _userManager.AddToRoleAsync(user, "Student");
                student.ApplicationUserId = user.Id;

                await _studentRepo.AddAsync(student);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var callbackUrl = Url.Action("SetPassword", "Account", new { userId = user.Id, token = encodedToken }, Request.Scheme);

                var body = $@"<p>Hello {student.FullName},</p>
                            <p>Your account has been created. To set your password, click on the link below:</p>
                            <p><a href='{callbackUrl}'>Set Password</a></p>";

                await _emailSender.SendEmailAsync(user.Email, "Set Password - EscolaInfoSys", body);

                TempData["Success"] = "Student created successfully. Password link sent by e-mail";
                return RedirectToAction(nameof(Index));
            }

            var fg = await _formGroupRepo.GetAllAsync();
            ViewData["FormGroupId"] = new SelectList(fg, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentRepo.GetByIdAsync(id.Value);
            if (student == null) return NotFound();

            var formGroups = await _formGroupRepo.GetAllAsync();
            ViewData["FormGroupId"] = new SelectList(formGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, IFormFile ProfilePhotoFile, IFormFile DocumentPhotoFile)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (ProfilePhotoFile != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(ProfilePhotoFile.FileName);
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await ProfilePhotoFile.CopyToAsync(stream);
                        student.ProfilePhoto = fileName;
                    }

                    if (DocumentPhotoFile != null)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(DocumentPhotoFile.FileName);
                        var path = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                        using var stream = new FileStream(path, FileMode.Create);
                        await DocumentPhotoFile.CopyToAsync(stream);
                        student.DocumentPhoto = fileName;
                    }

                    await _studentRepo.UpdateAsync(student);
                }
                catch
                {
                    if (!await _studentRepo.ExistsAsync(student.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var formGroups = await _formGroupRepo.GetAllAsync();
            ViewData["FormGroupId"] = new SelectList(formGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _studentRepo.GetWithFormGroupAsync(id.Value);
            if (student == null) return NotFound();

            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            if (student != null)
                await _studentRepo.DeleteAsync(student);

            return RedirectToAction(nameof(Index));
        }
    }
}

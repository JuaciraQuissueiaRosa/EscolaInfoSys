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

namespace EscolaInfoSys.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = _context.Students.Include(s => s.FormGroup);
            return View(await students.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Absences)
                .Include(s => s.Marks)
                .Include(s => s.FormGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            var exclusions = await _context.StudentExclusions
                .Where(e => e.StudentId == student.Id)
                .Include(e => e.Subject)
                .ToListAsync();

            ViewBag.Exclusions = exclusions;

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            ViewData["FormGroupId"] = new SelectList(_context.FormGroups, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student, IFormFile ProfilePhotoFile, IFormFile DocumentPhotoFile)
        {
            if (ModelState.IsValid)
            {
                // Upload de fotos
                if (ProfilePhotoFile != null && ProfilePhotoFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhotoFile.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await ProfilePhotoFile.CopyToAsync(stream);
                    student.ProfilePhoto = fileName;
                }

                if (DocumentPhotoFile != null && DocumentPhotoFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DocumentPhotoFile.FileName);
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                    using var stream = new FileStream(filePath, FileMode.Create);
                    await DocumentPhotoFile.CopyToAsync(stream);
                    student.DocumentPhoto = fileName;
                }

                // Criar usuário Identity
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
                        ModelState.AddModelError(string.Empty, error.Description);

                    ViewData["FormGroupId"] = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
                    return View(student);
                }

                // Atribuir role de "Student"
                await _userManager.AddToRoleAsync(user, "Student");

                // Salvar aluno
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                // Enviar email com link de definição de senha
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var callbackUrl = Url.Action("SetPassword", "Account", new { userId = user.Id, token = encodedToken }, Request.Scheme);

                var message = $@"
            <p>Olá {student.FullName},</p>
            <p>Sua conta foi criada na plataforma EscolaInfoSys.</p>
            <p>Para definir sua senha, clique no link abaixo:</p>
            <p><a href='{callbackUrl}'>Definir Senha</a></p>";

                await _emailSender.SendEmailAsync(student.Email, "Definir senha - EscolaInfoSys", message);

                TempData["Success"] = "Aluno criado e email de definição de senha enviado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FormGroupId"] = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            ViewData["FormGroupId"] = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student, IFormFile ProfilePhotoFile, IFormFile DocumentPhotoFile)
        {
            if (id != student.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Atualizar fotos se novas forem fornecidas
                    if (ProfilePhotoFile != null && ProfilePhotoFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePhotoFile.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ProfilePhotoFile.CopyToAsync(stream);
                        }
                        student.ProfilePhoto = fileName;
                    }

                    if (DocumentPhotoFile != null && DocumentPhotoFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DocumentPhotoFile.FileName);
                        var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await DocumentPhotoFile.CopyToAsync(stream);
                        }
                        student.DocumentPhoto = fileName;
                    }

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["FormGroupId"] = new SelectList(_context.FormGroups, "Id", "Name", student.FormGroupId);
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.FormGroup)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null) _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }

}

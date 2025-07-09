using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using EscolaInfoSys.Services;

namespace EscolaInfoSys.Controllers
{
    public class StaffMembersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public StaffMembersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        // GET: StaffMembers
        public async Task<IActionResult> Index()
        {
            return View(await _context.StaffMembers.ToListAsync());
        }

        // GET: StaffMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }

            return View(staffMember);
        }

        // GET: StaffMembers/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Create(StaffMember staffMember)
        {
            if (ModelState.IsValid)
            {
                // 1. Cria o ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = staffMember.Email,
                    Email = staffMember.Email,
                    EmailConfirmed = false
                };

                var userResult = await _userManager.CreateAsync(user);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    return View(staffMember);
                }

                // 2. Atribui a Role Staff
                await _userManager.AddToRoleAsync(user, "Staff");

                // 3. Salva o StaffMember com o Id do ApplicationUser
                staffMember.ApplicationUserId = user.Id;
                _context.Add(staffMember);
                await _context.SaveChangesAsync();

                // 4. Gera link de definição de senha
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var callbackUrl = Url.Action("SetPassword", "Account", new { userId = user.Id, token = encodedToken }, protocol: Request.Scheme);

                var body = $"<h3>Bem-vindo à plataforma!</h3><p>Por favor <a href='{callbackUrl}'>clique aqui</a> para definir sua senha de acesso.</p>";

                await _emailSender.SendEmailAsync(user.Email, "Definir senha de acesso", body);

                TempData["Success"] = "Funcionário criado com sucesso. O link de acesso foi enviado por e-mail.";
                return RedirectToAction(nameof(Index));
            }

            return View(staffMember);
        }


        // POST: StaffMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email")] StaffMember staffMember)
        {
            if (id != staffMember.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staffMember);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffMemberExists(staffMember.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staffMember);
        }

        // GET: StaffMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staffMember = await _context.StaffMembers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staffMember == null)
            {
                return NotFound();
            }

            return View(staffMember);
        }

        // POST: StaffMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staffMember = await _context.StaffMembers.FindAsync(id);
            if (staffMember != null)
            {
                _context.StaffMembers.Remove(staffMember);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffMemberExists(int id)
        {
            return _context.StaffMembers.Any(e => e.Id == id);
        }
    }
}

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
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StaffMembersController : Controller
    {
        private readonly IStaffMemberRepository _staffRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public StaffMembersController(IStaffMemberRepository staffRepo, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _staffRepo = staffRepo;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _staffRepo.GetAllAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _staffRepo.GetByIdAsync(id.Value);
            if (staff == null) return NotFound();

            return View(staff);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(StaffMember staffMember)
        {
            if (!ModelState.IsValid)
                return View(staffMember);

            var user = new ApplicationUser
            {
                UserName = staffMember.Email,
                Email = staffMember.Email,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(staffMember);
            }

            await _userManager.AddToRoleAsync(user, "Staff");

            staffMember.ApplicationUserId = user.Id;
            await _staffRepo.AddAsync(staffMember);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var callbackUrl = Url.Action("SetPassword", "Account", new { userId = user.Id, token = encodedToken }, protocol: Request.Scheme);

            var body = $"<h3>Bem-vindo à plataforma!</h3><p>Por favor <a href='{callbackUrl}'>clique aqui</a> para definir sua senha de acesso.</p>";
            await _emailSender.SendEmailAsync(user.Email, "Definir senha de acesso", body);

            TempData["Success"] = "Funcionário criado com sucesso. O link de acesso foi enviado por e-mail.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email")] StaffMember staffMember)
        {
            if (id != staffMember.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(staffMember);

            try
            {
                await _staffRepo.UpdateAsync(staffMember);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _staffRepo.ExistsAsync(staffMember.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var staff = await _staffRepo.GetByIdAsync(id.Value);
            if (staff == null) return NotFound();

            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _staffRepo.GetByIdAsync(id);
            if (staff != null)
                await _staffRepo.DeleteAsync(staff);

            return RedirectToAction(nameof(Index));
        }
    }

}

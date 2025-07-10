using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "StaffMember")]
    public class AlertsController : Controller
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IStaffMemberRepository _staffRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlertsController(
            IAlertRepository alertRepository,
            IStaffMemberRepository staffRepository,
            UserManager<ApplicationUser> userManager)
        {
            _alertRepository = alertRepository;
            _staffRepository = staffRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Alert alert)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                var staffMember = await _staffRepository.GetByApplicationUserIdAsync(userId);
                if (staffMember == null)
                {
                    ModelState.AddModelError("", "Staff member not found.");
                    return View(alert);
                }

                alert.StaffId = staffMember.Id;
                alert.StaffMember = staffMember;
                alert.CreatedAt = DateTime.Now;

                await _alertRepository.AddAsync(alert);
                return RedirectToAction("Index", "Home");
            }

            return View(alert);
        }
    }
}

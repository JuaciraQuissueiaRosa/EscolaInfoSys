using EscolaInfoSys.Data;
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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AlertsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
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

                var staffMember = await _context.StaffMembers
                    .Include(s => s.ApplicationUser)
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);

                if (staffMember == null)
                {
                    ModelState.AddModelError("", "Staff member not found.");
                    return View(alert);
                }

                alert.StaffId = staffMember.Id;
                alert.StaffMember = staffMember; 
                alert.CreatedAt = DateTime.Now;

                _context.Alerts.Add(alert);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(alert);
        }





    }
}

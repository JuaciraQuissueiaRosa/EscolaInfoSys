using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Hubs;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize]
    public class AlertsController : Controller
    {
        private readonly IAlertRepository _alertRepository;
        private readonly IStaffMemberRepository _staffRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AlertsController(
            IAlertRepository alertRepository,
            IStaffMemberRepository staffRepository,
            UserManager<ApplicationUser> userManager,
            IHubContext<NotificationHub> hubContext)
        {
            _alertRepository = alertRepository;
            _staffRepository = staffRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var alerts = await _alertRepository.GetAllWithStaffAsync();
            return View(alerts);
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

                // Envia a notificação ao grupo de admins via SignalR
                var fullName = staffMember.ApplicationUser?.Name ?? "Unknown";
                var email = staffMember.ApplicationUser?.Email ?? "no-email";
                var date = alert.CreatedAt.ToString("dd/MM/yyyy");

                var message = $"{alert.Title}<br>{alert.Message}<br>{fullName} ({email}) - {date}";

                await _hubContext.Clients.Group("Admins").SendAsync("ReceiveNotification", message, "warning");

                return RedirectToAction("Index", "Home");
            }

            return View(alert);
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null)
                return NotFound();

            alert.IsResolved = !alert.IsResolved;
            await _alertRepository.UpdateAsync(alert);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> MarkAsResolvedAjax(int id)
        {
            var alert = await _alertRepository.GetByIdAsync(id);
            if (alert == null) return NotFound();

            alert.IsResolved = true;
            await _alertRepository.UpdateAsync(alert);

            // TODO: Notificar via SignalR o usuário
            await _hubContext.Clients.All.SendAsync("AlertStatusChanged", id); // ← vamos configurar isso

            return Ok();
        }

        [Authorize(Roles = "StaffMember")]
        public async Task<IActionResult> MyAlerts()
        {
            var userId = _userManager.GetUserId(User);
            var staff = await _staffRepository.GetByApplicationUserIdAsync(userId);
            if (staff == null) return NotFound();

            var myAlerts = await _alertRepository.GetByStaffIdAsync(staff.Id);
            return View(myAlerts);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Respond(int id, string adminResponse)
        {
            var alert = await _alertRepository.GetByIdWithStaffAsync(id); // Usa método que inclui ApplicationUser
            if (alert == null)
                return Json(new { success = false, message = "Alert not found." });

            if (string.IsNullOrWhiteSpace(adminResponse))
                return Json(new { success = false, message = "Response is empty." });

            alert.AdminResponse = adminResponse;
            alert.IsResolved = true;
            await _alertRepository.UpdateAsync(alert);

            var staffUserId = alert.StaffMember?.ApplicationUser?.Id;

            if (!string.IsNullOrEmpty(staffUserId))
            {
                await _hubContext.Clients.User(staffUserId).SendAsync(
                    "ReceiveNotification",
                    $"Admin responded to your alert: '{alert.Title}'",
                    "info"
                );
            }

            return Json(new { success = true, id = alert.Id });
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetNewAlerts()
        {
            var alerts = await _alertRepository.GetAllWithStaffAsync();
            var unresolved = alerts.Where(a => !a.IsResolved).ToList();

            return PartialView("_AlertsPartial", unresolved);
        }






    }
}

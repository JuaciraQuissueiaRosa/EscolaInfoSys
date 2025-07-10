using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class SystemSettingsController : Controller
    {
        private readonly ISystemSettingsRepository _settingsRepo;

        public SystemSettingsController(ISystemSettingsRepository settingsRepo)
        {
            _settingsRepo = settingsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var settings = await _settingsRepo.GetSettingsAsync();

            // Se não existir, cria novo
            if (settings == null)
            {
                settings = new SystemSettings();
                await _settingsRepo.UpdateAsync(settings); // Adiciona novo
            }

            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SystemSettings model)
        {
            var settings = await _settingsRepo.GetSettingsAsync();
            if (settings == null)
                return NotFound();

            settings.MaxAbsencePercentage = model.MaxAbsencePercentage;

            await _settingsRepo.SaveAsync();
            return RedirectToAction("Dashboard", "Home");
        }
    }

}

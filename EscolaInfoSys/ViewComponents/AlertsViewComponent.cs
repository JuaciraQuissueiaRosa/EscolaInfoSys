using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.ViewComponents
{
    public class AlertsViewComponent : ViewComponent
    {
        private readonly IAlertRepository _alertRepository;

        public AlertsViewComponent(IAlertRepository alertRepository)
        {
            _alertRepository = alertRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var pendingAlerts = (await _alertRepository.GetAllAsync())
                .Where(a => !a.IsResolved && string.IsNullOrEmpty(a.AdminResponse))
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToList(); 
            return View(pendingAlerts);
        }

    }

}

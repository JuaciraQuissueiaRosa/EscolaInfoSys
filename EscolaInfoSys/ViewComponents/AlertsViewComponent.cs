using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.ViewComponents
{
    public class AlertsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public AlertsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var alerts = await _context.Alerts
                .Include(a => a.StaffMember) 
                .ThenInclude(sm => sm.ApplicationUser) 
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync();

            return View(alerts);
        }

    }
}

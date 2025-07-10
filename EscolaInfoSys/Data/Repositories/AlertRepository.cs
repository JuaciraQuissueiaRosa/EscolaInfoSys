using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly ApplicationDbContext _context;

        public AlertRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Alert alert)
        {
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Alert>> GetAllAsync()
        {
            return await _context.Alerts
                .Include(a => a.StaffMember)
                .ThenInclude(s => s.ApplicationUser)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Alert?> GetByIdAsync(int id)
        {
            return await _context.Alerts
                .Include(a => a.StaffMember)
                .ThenInclude(s => s.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}

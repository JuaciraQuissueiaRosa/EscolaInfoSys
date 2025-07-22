using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class AlertRepository : GenericRepository<Alert>, IAlertRepository
    {
        public AlertRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Alert>> GetAllWithStaffAsync()
        {
            return await _context.Alerts
                .Include(a => a.StaffMember)
                .ThenInclude(s => s.ApplicationUser)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<Alert?> GetByIdWithStaffAsync(int id)
        {
            return await _context.Alerts
                .Include(a => a.StaffMember)
                .ThenInclude(s => s.ApplicationUser)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Alert>> GetByStaffIdAsync(int staffId)
        {
            return await _context.Alerts
                .Where(a => a.StaffId == staffId)
                .Include(a => a.StaffMember)
                .ThenInclude(s => s.ApplicationUser)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }


}

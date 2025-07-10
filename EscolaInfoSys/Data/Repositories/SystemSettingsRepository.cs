using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class SystemSettingsRepository : ISystemSettingsRepository
    {
        private readonly ApplicationDbContext _context;
        public SystemSettingsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SystemSettings> GetSettingsAsync()
        {
            return await _context.SystemSettings.FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(SystemSettings settings)
        {
            _context.SystemSettings.Update(settings);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }


}

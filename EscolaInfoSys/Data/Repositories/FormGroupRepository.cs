using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class FormGroupRepository : IFormGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public FormGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormGroup>> GetAllAsync()
        {
            return await _context.FormGroups
                .Include(f => f.Students)
                .Include(f => f.Subjects)
                .ToListAsync();
        }

        public async Task<FormGroup?> GetByIdAsync(int id)
        {
            return await _context.FormGroups
                .Include(f => f.Students)
                .Include(f => f.Subjects)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddAsync(FormGroup formGroup)
        {
            _context.FormGroups.Add(formGroup);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FormGroup formGroup)
        {
            _context.FormGroups.Update(formGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(FormGroup formGroup)
        {
            _context.FormGroups.Remove(formGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.FormGroups.AnyAsync(f => f.Id == id);
        }
    }
}

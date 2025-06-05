using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class MarkRepository : IMarkRepository
    {
        private readonly ApplicationDbContext _context;

        public MarkRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mark>> GetAllAsync()
        {
            return await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .ToListAsync();
        }

        public async Task<Mark?> GetByIdAsync(int id)
        {
            return await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Mark mark)
        {
            _context.Marks.Add(mark);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Mark mark)
        {
            _context.Marks.Update(mark);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Mark mark)
        {
            _context.Marks.Remove(mark);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Marks.AnyAsync(m => m.Id == id);
        }
    }
}

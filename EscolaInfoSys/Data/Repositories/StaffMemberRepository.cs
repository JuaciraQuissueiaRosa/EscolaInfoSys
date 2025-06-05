using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class StaffMemberRepository : IStaffMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffMember>> GetAllAsync()
        {
            return await _context.StaffMembers.ToListAsync();
        }

        public async Task<StaffMember?> GetByIdAsync(int id)
        {
            return await _context.StaffMembers.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(StaffMember staff)
        {
            _context.StaffMembers.Add(staff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StaffMember staff)
        {
            _context.StaffMembers.Update(staff);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(StaffMember staff)
        {
            _context.StaffMembers.Remove(staff);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.StaffMembers.AnyAsync(s => s.Id == id);
        }
    }
}

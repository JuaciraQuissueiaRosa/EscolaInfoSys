using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class StaffMemberRepository : GenericRepository<StaffMember>, IStaffMemberRepository
    {
        public StaffMemberRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<StaffMember>> GetAllWithUserAsync()
        {
            return await _context.StaffMembers
                .Include(s => s.ApplicationUser)
                .ToListAsync();
        }

        public async Task<StaffMember?> GetByIdWithUserAsync(int id)
        {
            return await _context.StaffMembers
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StaffMember?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.StaffMembers
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId);
        }
    }


}

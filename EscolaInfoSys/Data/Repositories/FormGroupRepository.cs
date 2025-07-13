using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class FormGroupRepository : GenericRepository<FormGroup>, IFormGroupRepository
    {
        public FormGroupRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<FormGroup>> GetAllAsync()
        {
            return await _context.FormGroups
                .Include(f => f.Students)
                .Include(f => f.Subjects)
                .ToListAsync();
        }

        public override async Task<FormGroup?> GetByIdAsync(int id)
        {
            return await _context.FormGroups
                .Include(f => f.Students)
                .Include(f => f.Subjects)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }

}

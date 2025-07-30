using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class FormGroupRepository : GenericRepository<FormGroup>, IFormGroupRepository
    {
        public FormGroupRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<FormGroup>> GetAllWithStudentsAndSubjectsAsync()
        {
            return await _context.FormGroups
                .Include(fg => fg.Students)
                .Include(fg => fg.Course)
                    .ThenInclude(c => c.Subjects)
                .ToListAsync();
        }

        public async Task<FormGroup?> GetByIdWithStudentsAndSubjectsAsync(int id)
        {
            return await _context.FormGroups
                .Include(fg => fg.Students)
                .Include(fg => fg.Course)
                    .ThenInclude(c => c.Subjects)
                .FirstOrDefaultAsync(fg => fg.Id == id);
        }

    }


}

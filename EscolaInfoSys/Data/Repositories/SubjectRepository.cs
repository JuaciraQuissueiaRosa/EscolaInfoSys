using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Subject>> GetAllWithCourseAsync()
        {
            return await _context.Subjects
                .Include(s => s.Course)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdWithCourseAsync(int id)
        {
            return await _context.Subjects
                .Include(s => s.Course)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }

}

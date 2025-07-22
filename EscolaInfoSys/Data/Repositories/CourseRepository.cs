using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Course>> GetAllWithSubjectsAsync()
        {
            return await _context.Courses
                .Include(c => c.Subjects)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithSubjectsAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Subjects)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }


}

using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.Include(c => c.Subjects).ToListAsync();
        }

        public override async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Subjects)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

}

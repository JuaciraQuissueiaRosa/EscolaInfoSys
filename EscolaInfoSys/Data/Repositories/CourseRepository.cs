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
                  .Include(c => c.FormGroups)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdWithSubjectsAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Subjects)
                  .Include(c => c.FormGroups)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateCourseFormGroupsAsync(int courseId, IEnumerable<int> formGroupIds)
        {
            var course = await GetByIdWithSubjectsAsync(courseId);
            if (course == null) throw new Exception("Course not found");

            // Buscar FormGroups direto no contexto
            var selectedFormGroups = await _context.FormGroups
                .Where(fg => formGroupIds.Contains(fg.Id))
                .ToListAsync();

            // Limpar associações antigas
            course.FormGroups.Clear();

            // Adicionar as novas
            foreach (var fg in selectedFormGroups)
            {
                course.FormGroups.Add(fg);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int[]> GetFormGroupIdsByCourseIdAsync(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.FormGroups)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return Array.Empty<int>();

            return course.FormGroups.Select(fg => fg.Id).ToArray();
        }


    }


}

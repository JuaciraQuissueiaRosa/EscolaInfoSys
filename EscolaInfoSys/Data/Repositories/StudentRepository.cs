using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Student>> GetWithFormGroupAsync()
        {
            return await _context.Students
                .Include(s => s.FormGroup)
                .ToListAsync();
        }

        public async Task<Student?> GetWithFormGroupAsync(int id)
        {
            return await _context.Students
                .Include(s => s.FormGroup)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetFullByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.FormGroup)
                .Include(s => s.Marks)
                    .ThenInclude(m => m.Subject)
                .Include(s => s.Absences)
                    .ThenInclude(a => a.Subject)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<StudentExclusion>> GetExclusionsAsync(int studentId)
        {
            return await _context.StudentExclusions
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Subject)
                .ToListAsync();
        }

        public async Task<Student?> GetByApplicationUserIdAsync(string applicationUserId)
        {
            return await _context.Students
                .Include(s => s.FormGroup)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == applicationUserId);
        }

    }


}

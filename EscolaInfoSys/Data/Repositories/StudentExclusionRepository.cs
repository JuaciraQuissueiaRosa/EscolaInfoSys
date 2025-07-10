using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class StudentExclusionRepository : IStudentExclusionRepository
    {
        private readonly ApplicationDbContext _context;
        public StudentExclusionRepository(ApplicationDbContext context) => _context = context;

        public async Task<StudentExclusion?> GetByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.StudentExclusions
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);
        }

        public async Task AddAsync(StudentExclusion exclusion)
        {
            _context.StudentExclusions.Add(exclusion);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentExclusion>> GetAllAsync()
        {
            return await _context.StudentExclusions.ToListAsync();
        }
    }

}

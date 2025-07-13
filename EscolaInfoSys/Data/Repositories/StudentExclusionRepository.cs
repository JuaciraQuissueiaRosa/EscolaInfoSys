using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class StudentExclusionRepository : GenericRepository<StudentExclusion>, IStudentExclusionRepository
    {
        public StudentExclusionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<StudentExclusion?> GetByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.StudentExclusions
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);
        }

      
    }


}

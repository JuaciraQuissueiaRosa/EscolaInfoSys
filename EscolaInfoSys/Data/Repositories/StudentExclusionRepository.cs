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

        public async Task<List<StudentExclusion>> GetByStudentAndSubjectIdsAsync(
         IEnumerable<int> studentIds,
          IEnumerable<int> subjectIds)
        {
            return await _context.StudentExclusions
                .Where(e => studentIds.Contains(e.StudentId) && subjectIds.Contains(e.SubjectId))
                .ToListAsync();
        }

        public async Task<(IEnumerable<int> studentIds, IEnumerable<int> subjectIds)> GetExcludedStudentAndSubjectIdsAsync()
        {
            var exclusions = await _context.StudentExclusions.ToListAsync();

            var studentIds = exclusions.Select(e => e.StudentId).Distinct();
            var subjectIds = exclusions.Select(e => e.SubjectId).Distinct();

            return (studentIds, subjectIds);
        }


    }


}

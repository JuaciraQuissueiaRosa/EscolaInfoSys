using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class AbsenceRepository : GenericRepository<Absence>, IAbsenceRepository
    {
        public AbsenceRepository(ApplicationDbContext context) : base(context) { }

        public async Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.Absences
                .CountAsync(a => a.StudentId == studentId && a.SubjectId == subjectId);
        }
    }

}

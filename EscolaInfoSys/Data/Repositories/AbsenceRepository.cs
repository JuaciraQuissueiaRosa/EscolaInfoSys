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
        public override async Task<IEnumerable<Absence>> GetAllAsync()
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .ToListAsync();
        }

        public async Task<Absence?> GetByIdWithStudentAndSubjectAsync(int id)
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Absence?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Absence>> GetAllWithStudentAndSubjectAsync()
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .ToListAsync();
        }




    }

}

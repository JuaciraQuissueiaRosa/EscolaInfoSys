using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class AbsenceRepository : IAbsenceRepository
    {
        private readonly ApplicationDbContext _context;

        public AbsenceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Absence>> GetAllAsync()
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .ToListAsync();
        }

        public async Task<Absence?> GetByIdAsync(int id)
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAsync(Absence absence)
        {
            _context.Absences.Add(absence);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Absence absence)
        {
            _context.Absences.Update(absence);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Absence absence)
        {
            _context.Absences.Remove(absence);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Absences.AnyAsync(a => a.Id == id);
        }

        public async Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.Absences
                .CountAsync(a => a.StudentId == studentId && a.SubjectId == subjectId);
        }

    }
}

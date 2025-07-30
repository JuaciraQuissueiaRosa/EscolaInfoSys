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
                   .Include(s => s.Course)
                .ToListAsync();
        }

        public async Task<Student?> GetWithFormGroupAsync(int id)
        {
            return await _context.Students
                .Include(s => s.FormGroup)
                .Include(s=>s.Course)
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
                    .Include(a=>a.Course)
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

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            return await _context.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
        }

        public async Task UpdateSelectedFieldsAsync(Student student)
        {
            var existing = await _context.Students.FindAsync(student.Id);
            if (existing == null) return;

            // Atualização de campos permitidos
            existing.FullName = student.FullName;
            existing.PupilNumber = student.PupilNumber;
            existing.Email = student.Email;
            existing.FormGroupId = student.FormGroupId;
            existing.ProfilePhoto = student.ProfilePhoto;
            existing.DocumentPhoto = student.DocumentPhoto;

            // Mantém o ApplicationUserId existente (caso tenha sido apagado por acidente)
            if (!string.IsNullOrEmpty(student.ApplicationUserId))
                existing.ApplicationUserId = student.ApplicationUserId;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsStudentExcludedFromAnySubjectAsync(int studentId)
        {
            return await _context.StudentExclusions.AnyAsync(se => se.StudentId == studentId);
        }

    }


}

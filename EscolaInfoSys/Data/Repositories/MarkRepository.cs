using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class MarkRepository : GenericRepository<Mark>, IMarkRepository
    {
        public MarkRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Mark>> GetAllAsync()
        {
            return await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                    .ThenInclude(s => s.ApplicationUser)
                .ToListAsync();
        }


        public override async Task<Mark?> GetByIdAsync(int id)
        {
            return await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                    .ThenInclude(s => s.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Mark>> GetValidMarksAsync(IEnumerable<int> excludedStudentIds, IEnumerable<int> excludedSubjectIds)
        {
            var query = _context.Marks.AsQueryable();

            if (excludedStudentIds.Any() && excludedSubjectIds.Any())
            {
                query = query.Where(m =>
                    !(m.StudentId.HasValue && m.SubjectId.HasValue &&
                      excludedStudentIds.Contains(m.StudentId.Value) &&
                      excludedSubjectIds.Contains(m.SubjectId.Value)));
            }

            return await query
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                .ToListAsync();
        }

    }

}

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
            var excludedStudentIdsNullable = excludedStudentIds.Select(id => (int?)id).ToList();
            var excludedSubjectIdsNullable = excludedSubjectIds.Select(id => (int?)id).ToList();

            var query = _context.Marks.AsQueryable();

            //if (excludedStudentIds.Any() || excludedSubjectIds.Any())
            //{
            //    query = query.Where(m =>
            //        !excludedStudentIdsNullable.Contains(m.StudentId)
            //        && !excludedSubjectIdsNullable.Contains(m.SubjectId)
            //    );
            //}

            return await query
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.StaffMember)
                    .ThenInclude(s => s.ApplicationUser)
                .ToListAsync();
        }


    }

}

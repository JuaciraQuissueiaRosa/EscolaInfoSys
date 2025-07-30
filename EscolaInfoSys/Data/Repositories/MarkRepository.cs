using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using EscolaInfoSys.Models.Dtos;
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

        public async Task UpdateIsPassedStatusAsync()
        {
            var groupedMarks = await _context.Marks
                .GroupBy(m => new { m.StudentId, m.SubjectId })
                .ToListAsync();

            foreach (var group in groupedMarks)
            {
                var average = group.Average(m => m.Value);

                foreach (var mark in group)
                {
                    mark.IsPassed = average >= 10;
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentSubjectPassStatusAsync(int studentId, int subjectId)
        {
            // Busca as notas do aluno para a disciplina específica
            var marks = await _context.Marks
                .Where(m => m.StudentId == studentId && m.SubjectId == subjectId)
                .ToListAsync();

            if (!marks.Any())
                return; // Sem notas, nada a fazer

            // Calcula a média das notas
            var average = marks.Average(m => m.Value);

            // Define se passou (média >= 10)
            bool isPassed = average >= 10;

            // Atualiza o campo IsPassed em todas as notas dessa combinação
            foreach (var mark in marks)
            {
                mark.IsPassed = isPassed;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<StudentSubjectAverageDto>> GetStudentSubjectAveragesByStaffAsync(int staffId)
        {
            return await _context.Marks
                .Where(m => m.StaffMemberId == staffId)
                .GroupBy(m => new { m.StudentId, m.Student.Email, m.SubjectId, m.Subject.Name })
                .Select(g => new StudentSubjectAverageDto
                {
                    StudentId = g.Key.StudentId,
                    StudentEmail = g.Key.Email,
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.Name,
                    Average = g.Average(m => m.Value),
                    IsPassed = g.Average(m => m.Value) >= 10
                }).ToListAsync();
        }

        public async Task<List<StudentSubjectAverageDto>> GetStudentSubjectAveragesAsync(int studentId)
        {
            return await _context.Marks
                .Where(m => m.StudentId == studentId)
                .GroupBy(m => new { m.SubjectId, m.Subject.Name })
                .Select(g => new StudentSubjectAverageDto
                {
                    SubjectId = g.Key.SubjectId,
                    SubjectName = g.Key.Name,
                    Average = g.Average(m => m.Value),
                    IsPassed = g.Average(m => m.Value) >= 10
                }).ToListAsync();
        }

   




    }

}

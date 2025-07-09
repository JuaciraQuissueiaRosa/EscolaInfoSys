using EscolaInfoSys.Data;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Services
{
    public class AbsenceCheckerService
    {
        private readonly ApplicationDbContext _context;

        public AbsenceCheckerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CheckExclusionAsync(int studentId, int subjectId)
        {
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null || subject.TotalLessons == 0)
                return;

            var absences = await _context.Absences
                .CountAsync(a => a.StudentId == studentId && a.SubjectId == subjectId);

            var settings = await _context.SystemSettings.FirstAsync();
            double percentage = (double)absences / subject.TotalLessons * 100.0;

            var exclusion = await _context.StudentExclusions
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.SubjectId == subjectId);

            if (exclusion == null)
            {
                exclusion = new StudentExclusion
                {
                    StudentId = studentId,
                    SubjectId = subjectId
                };
                _context.StudentExclusions.Add(exclusion);
            }

            exclusion.IsExcluded = percentage >= settings.MaxAbsencePercentage;
            await _context.SaveChangesAsync();
        }
    }


}

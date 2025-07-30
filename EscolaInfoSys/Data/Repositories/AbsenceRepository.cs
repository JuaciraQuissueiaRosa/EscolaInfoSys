using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using EscolaInfoSys.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Data.Repositories
{
    public class AbsenceRepository : GenericRepository<Absence>, IAbsenceRepository
    {
        ISystemSettingsRepository _settingsRepo;


        public AbsenceRepository(ApplicationDbContext context, ISystemSettingsRepository settingsRepo)
            : base(context)
        {
            _settingsRepo = settingsRepo;
        }

        public async Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.Absences
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && !a.Justified)
                .CountAsync();
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

        public async Task<IEnumerable<Absence>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Absences
                .Include(a => a.Subject)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<(List<Absence> Absences, double Percentage, double Limit)> GetAbsencesWithStatsAsync(int studentId)
        {
            var absences = (await GetByStudentIdAsync(studentId)).ToList();

            var settings = await _settingsRepo.GetSettingsAsync();
            double maxAllowedAbsences = settings?.MaxAbsencePercentage ?? 15.0;

            int totalAbsences = absences.Count;
            double percentage = maxAllowedAbsences == 0 ? 0 : (100.0 * totalAbsences / maxAllowedAbsences);
            percentage = Math.Min(percentage, 150);

            return (absences, Math.Round(percentage, 1), maxAllowedAbsences);
        }

        public async Task<IEnumerable<Absence>> GetAbsencesByStudentIdAsync(int studentId)
        {
            return await _context.Absences
            .Include(a => a.Subject)  
            .Where(a => a.StudentId == studentId)
            .ToListAsync();
        }

        public async Task<IEnumerable<AbsenceViewModel>> GetAbsenceViewModelsByStudentIdAsync(int studentId)
        {
            return await _context.Absences
                .Include(a => a.Subject)
                .Where(a => a.StudentId == studentId)
                .Select(a => new AbsenceViewModel
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    SubjectId = a.SubjectId,
                    SubjectName = a.Subject.Name,
                    Date = a.Date,
                    Justified = a.Justified
                })
                .ToListAsync();
        }

        public async Task<int> CountUnjustifiedAbsencesByStudentAndSubjectAsync(int studentId, int subjectId)
        {
            return await _context.Absences
                .Where(a => a.StudentId == studentId && a.SubjectId == subjectId && !a.Justified)
                .CountAsync();
        }


        public async Task UpdateFromViewModelAsync(AbsenceEditViewModel model)
        {
            var absence = await GetByIdAsync(model.Id);
            if (absence == null) throw new Exception("Absence not found");

            absence.StudentId = model.StudentId;
            absence.SubjectId = model.SubjectId;
            absence.Date = model.Date;
            absence.Justified = model.Justified;

            await UpdateAsync(absence);
        }


        public async Task<AbsenceEditViewModel?> GetAbsenceEditViewModelAsync(int id)
        {
            var absence = await _context.Absences
                .Include(a => a.Student)  // caso precise dados extras
                .Include(a => a.Subject)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (absence == null) return null;

            return new AbsenceEditViewModel
            {
                Id = absence.Id,
                StudentId = absence.StudentId,
                SubjectId = absence.SubjectId,
                Date = absence.Date,
                Justified = absence.Justified
            };
        }


    }

}

using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models.Dtos;
using EscolaInfoSys.Models.ViewModels;

namespace EscolaInfoSys.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAbsenceRepository _absenceRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly ISystemSettingsRepository _settingsRepo;

        public StudentService(IStudentRepository studentRepo, IAbsenceRepository absenceRepo, ISubjectRepository subjectRepo, ISystemSettingsRepository settingsRepo)
        {
            _studentRepo = studentRepo;
            _absenceRepo = absenceRepo;
            _subjectRepo = subjectRepo;
            _settingsRepo = settingsRepo;
        }

        public async Task<(List<AbsenceViewModel> Absences, bool IsExcluded)> GetStudentAbsencesAndExclusionAsync(string userId)
        {
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null) return (new List<AbsenceViewModel>(), false);

            var isExcluded = await _studentRepo.IsStudentExcludedFromAnySubjectAsync(student.Id);

            var absences = await _absenceRepo.GetByStudentIdAsync(student.Id);

            var viewModel = absences.Select(a => new AbsenceViewModel
            {
                SubjectName = a.Subject.Name,
                Date = a.Date,
                Justified = a.Justified
            }).ToList();

            return (viewModel, isExcluded);
        }

        public async Task<List<StudentAbsenceStatusDto>> GetStudentAbsencesWithExclusionStatusAsync(int studentId)
        {
            var absences = await _absenceRepo.GetByStudentIdAsync(studentId);
            var settings = await _settingsRepo.GetSettingsAsync();
            double maxAllowedPercentage = settings?.MaxAbsencePercentage ?? 15.0;

            var absencesGrouped = absences
                .GroupBy(a => a.SubjectId)
                .Select(g => new
                {
                    SubjectId = g.Key,
                    SubjectName = g.First().Subject.Name,
                    UnjustifiedAbsencesCount = g.Count(a => !a.Justified)
                }).ToList();

            var result = new List<StudentAbsenceStatusDto>();

            foreach (var group in absencesGrouped)
            {
                int totalClasses = await _subjectRepo.GetTotalClassesBySubjectIdAsync(group.SubjectId);

                double absencePercentage = totalClasses == 0 ? 0 : 100.0 * group.UnjustifiedAbsencesCount / totalClasses;

                bool isExcluded = absencePercentage > maxAllowedPercentage;

                result.Add(new StudentAbsenceStatusDto
                {
                    SubjectId = group.SubjectId,
                    SubjectName = group.SubjectName,
                    AbsencesCount = group.UnjustifiedAbsencesCount,
                    AbsencePercentage = absencePercentage,
                    IsExcluded = isExcluded
                });
            }

            return result;
        }

        public async Task<(List<StudentAbsenceStatusDto> AbsencesStatus, bool IsExcluded)> GetStudentAbsencesAndExclusionStatusAsync(string userId)
        {
            var student = await _studentRepo.GetByApplicationUserIdAsync(userId);
            if (student == null) return (new List<StudentAbsenceStatusDto>(), false);

            var absencesStatus = await GetStudentAbsencesWithExclusionStatusAsync(student.Id);

            bool isExcluded = absencesStatus.Any(a => a.IsExcluded);

            return (absencesStatus, isExcluded);
        }

    }

}

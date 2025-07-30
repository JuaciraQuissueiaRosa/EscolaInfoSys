using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models.ViewModels;

namespace EscolaInfoSys.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAbsenceRepository _absenceRepo;

        public StudentService(IStudentRepository studentRepo, IAbsenceRepository absenceRepo)
        {
            _studentRepo = studentRepo;
            _absenceRepo = absenceRepo;
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
    }

}

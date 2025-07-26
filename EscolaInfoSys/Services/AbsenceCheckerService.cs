using EscolaInfoSys.Data;
using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Services
{
    public class AbsenceCheckerService
    {
        private readonly ISubjectRepository _subjectRepo;
        private readonly IAbsenceRepository _absenceRepo;
        private readonly IStudentExclusionRepository _exclusionRepo;
        private readonly ISystemSettingsRepository _settingsRepo;

        public AbsenceCheckerService(
            ISubjectRepository subjectRepo,
            IAbsenceRepository absenceRepo,
            IStudentExclusionRepository exclusionRepo,
            ISystemSettingsRepository settingsRepo)
        {
            _subjectRepo = subjectRepo;
            _absenceRepo = absenceRepo;
            _exclusionRepo = exclusionRepo;
            _settingsRepo = settingsRepo;
        }

        public async Task CheckExclusionAsync(int studentId, int subjectId)
        {
            var subject = await _subjectRepo.GetByIdAsync(subjectId);
            if (subject == null || subject.TotalLessons == 0)
                return;

            var absences = await _absenceRepo.CountByStudentAndSubjectAsync(studentId, subjectId);
            if (absences == 0)
                return; //nenhuma falta registrada

            var settings = await _settingsRepo.GetSettingsAsync();
            double percentage = (double)absences / subject.TotalLessons * 100.0;

            var exclusion = await _exclusionRepo.GetByStudentAndSubjectAsync(studentId, subjectId);
            if (exclusion == null)
            {
                exclusion = new StudentExclusion
                {
                    StudentId = studentId,
                    SubjectId = subjectId
                };
                await _exclusionRepo.AddAsync(exclusion);
            }

            exclusion.IsExcluded = percentage >= settings.MaxAbsencePercentage;
            await _exclusionRepo.SaveAsync();
        }

    }


}

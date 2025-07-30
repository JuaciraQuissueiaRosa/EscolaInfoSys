using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Models;
using Microsoft.EntityFrameworkCore;

namespace EscolaInfoSys.Services
{
    public class AbsenceStatsService
    {
        private readonly IAbsenceRepository _absenceRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IStudentExclusionRepository _exclusionRepo;
        private readonly ISystemSettingsRepository _settingsRepo;

        public AbsenceStatsService(
            IAbsenceRepository absenceRepo,
            ISubjectRepository subjectRepo,
            IStudentExclusionRepository exclusionRepo,
            ISystemSettingsRepository settingsRepo)
        {
            _absenceRepo = absenceRepo;
            _subjectRepo = subjectRepo;
            _exclusionRepo = exclusionRepo;
            _settingsRepo = settingsRepo;
        }

        public async Task<AbsenceStatsResult> GetAbsenceStatsAsync()
        {
            var absences = await _absenceRepo.GetAllAsync();
            var subjects = await _subjectRepo.GetAllAsync();
            var exclusions = await _exclusionRepo.GetAllAsync();
            var settings = await _settingsRepo.GetSettingsAsync();

            var subjectDict = subjects.ToDictionary(s => s.Id, s => s.TotalLessons);
            var result = new AbsenceStatsResult
            {
                Absences = absences,
                Exclusions = exclusions,
                Percentages = new Dictionary<string, double>(),
                MaxAbsences = new Dictionary<int, int>()
            };

            foreach (var subject in subjects)
            {
                int max = (int)Math.Floor(subject.TotalLessons * settings.MaxAbsencePercentage / 100.0);
                result.MaxAbsences[subject.Id] = max;
            }

            foreach (var a in absences)
            {
                if (a.StudentId == null || a.SubjectId == null) continue;

                var key = $"{a.StudentId}-{a.SubjectId}";

                if (!result.Percentages.ContainsKey(key) && subjectDict.TryGetValue(a.SubjectId, out int totalLessons) && totalLessons > 0)
                {
                    var count = await _absenceRepo.CountByStudentAndSubjectAsync(a.StudentId, a.SubjectId);
                    double pct = (double)count / totalLessons * 100.0;
                    result.Percentages[key] = Math.Round(pct, 1);
                }

            }

            return result;
        }

       
    }




}

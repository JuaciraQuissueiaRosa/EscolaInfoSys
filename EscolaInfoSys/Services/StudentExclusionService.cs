using EscolaInfoSys.Data.Repositories.Interfaces;
using EscolaInfoSys.Hubs;
using EscolaInfoSys.Models;
using Microsoft.AspNetCore.SignalR;

namespace EscolaInfoSys.Services
{
    public class StudentExclusionService
    {
        private readonly IStudentExclusionRepository _exclusionRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ISubjectRepository _subjectRepo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public StudentExclusionService(
            IStudentExclusionRepository exclusionRepo,
            IStudentRepository studentRepo,
            ISubjectRepository subjectRepo,
            IHubContext<NotificationHub> hubContext)
        {
            _exclusionRepo = exclusionRepo;
            _studentRepo = studentRepo;
            _subjectRepo = subjectRepo;
            _hubContext = hubContext;
        }

        public async Task ExcludeStudentAsync(int studentId, int subjectId)
        {
            var student = await _studentRepo.GetByIdAsync(studentId);
            var subject = await _subjectRepo.GetByIdAsync(subjectId);
            if (student == null || subject == null) return;

            var existing = await _exclusionRepo.GetByStudentAndSubjectAsync(studentId, subjectId);
            if (existing != null)
            {
                existing.IsExcluded = true;
                await _exclusionRepo.UpdateAsync(existing);
            }
            else
            {
                var exclusion = new StudentExclusion
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    IsExcluded = true
                };
                await _exclusionRepo.AddAsync(exclusion);
            }
        }

    }
}


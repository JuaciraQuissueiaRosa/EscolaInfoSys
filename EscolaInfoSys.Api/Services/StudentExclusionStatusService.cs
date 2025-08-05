using EscolaInfoSys.Data.Repositories.Interfaces;

namespace EscolaInfoSys.Api.Services
{
    public class StudentExclusionStatusService : IStudentExclusionStatusService
    {
        private readonly IStudentExclusionRepository _exclusionRepo;

        public StudentExclusionStatusService(IStudentExclusionRepository exclusionRepo)
        {
            _exclusionRepo = exclusionRepo;
        }

        public async Task<bool> IsStudentExcludedFromSubjectAsync(int studentId, int subjectId)
        {
            var exclusion = await _exclusionRepo.GetByStudentAndSubjectAsync(studentId, subjectId);
            return exclusion != null;
        }

        public async Task<(IEnumerable<int> StudentIds, IEnumerable<int> SubjectIds)> GetAllExclusionsAsync()
        {
            return await _exclusionRepo.GetExcludedStudentAndSubjectIdsAsync();
        }
    }

}

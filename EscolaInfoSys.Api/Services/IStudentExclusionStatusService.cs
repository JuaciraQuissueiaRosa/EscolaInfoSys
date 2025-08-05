namespace EscolaInfoSys.Api.Services
{
    public interface IStudentExclusionStatusService
    {
        Task<bool> IsStudentExcludedFromSubjectAsync(int studentId, int subjectId);
        Task<(IEnumerable<int> StudentIds, IEnumerable<int> SubjectIds)> GetAllExclusionsAsync();
    }
}

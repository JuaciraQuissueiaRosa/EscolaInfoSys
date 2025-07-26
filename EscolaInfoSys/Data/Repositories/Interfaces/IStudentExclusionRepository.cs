using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStudentExclusionRepository : IGenericRepository<StudentExclusion>
    {
        Task<StudentExclusion?> GetByStudentAndSubjectAsync(int studentId, int subjectId);

        Task<List<StudentExclusion>> GetByStudentAndSubjectIdsAsync(
       IEnumerable<int> studentIds,
       IEnumerable<int> subjectIds);

        Task<(IEnumerable<int> studentIds, IEnumerable<int> subjectIds)> GetExcludedStudentAndSubjectIdsAsync();




    }


}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStudentExclusionRepository
    {
        Task<StudentExclusion?> GetByStudentAndSubjectAsync(int studentId, int subjectId);
        Task AddAsync(StudentExclusion exclusion);
        Task SaveAsync();

        Task<IEnumerable<StudentExclusion>> GetAllAsync();
    }

}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStudentExclusionRepository : IGenericRepository<StudentExclusion>
    {
        Task<StudentExclusion?> GetByStudentAndSubjectAsync(int studentId, int subjectId);

     
    }


}

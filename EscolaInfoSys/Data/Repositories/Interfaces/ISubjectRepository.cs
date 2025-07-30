using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<IEnumerable<Subject>> GetAllWithCourseAsync();
        Task<Subject?> GetByIdWithCourseAsync(int id);

        Task<IEnumerable<Subject>> GetByIdsAsync(IEnumerable<int> ids);

        Task<int> GetTotalClassesBySubjectIdAsync(int subjectId);

    }


}

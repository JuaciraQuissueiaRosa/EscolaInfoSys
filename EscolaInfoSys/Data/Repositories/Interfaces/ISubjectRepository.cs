using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface ISubjectRepository : IGenericRepository<Subject>
    {
        Task<IEnumerable<Subject>> GetAllWithCourseAsync();
        Task<Subject?> GetByIdWithCourseAsync(int id);
    }


}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllWithSubjectsAsync();
        Task<Course?> GetByIdWithSubjectsAsync(int id);
    }


}

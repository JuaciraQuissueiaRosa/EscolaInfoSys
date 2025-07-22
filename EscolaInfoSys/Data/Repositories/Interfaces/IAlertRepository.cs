using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IAlertRepository : IGenericRepository<Alert>
    {
        Task<IEnumerable<Alert>> GetAllWithStaffAsync();
        Task<Alert?> GetByIdWithStaffAsync(int id);
        Task<IEnumerable<Alert>> GetByStaffIdAsync(int staffId);
    }

}

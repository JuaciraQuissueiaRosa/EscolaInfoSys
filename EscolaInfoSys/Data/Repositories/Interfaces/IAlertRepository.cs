using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IAlertRepository
    {
        Task AddAsync(Alert alert);
        Task<IEnumerable<Alert>> GetAllAsync();
        Task<Alert?> GetByIdAsync(int id);
    }
}

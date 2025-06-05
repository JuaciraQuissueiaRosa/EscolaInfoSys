using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IMarkRepository
    {
        Task<IEnumerable<Mark>> GetAllAsync();
        Task<Mark?> GetByIdAsync(int id);
        Task AddAsync(Mark mark);
        Task UpdateAsync(Mark mark);
        Task DeleteAsync(Mark mark);
        Task<bool> ExistsAsync(int id);
    }
}

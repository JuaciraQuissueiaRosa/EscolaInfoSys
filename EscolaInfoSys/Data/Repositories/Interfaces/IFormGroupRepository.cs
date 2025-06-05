using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IFormGroupRepository
    {
        Task<IEnumerable<FormGroup>> GetAllAsync();
        Task<FormGroup?> GetByIdAsync(int id);
        Task AddAsync(FormGroup formGroup);
        Task UpdateAsync(FormGroup formGroup);
        Task DeleteAsync(FormGroup formGroup);
        Task<bool> ExistsAsync(int id);
    }
}

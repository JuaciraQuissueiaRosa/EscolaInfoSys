using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IFormGroupRepository : IGenericRepository<FormGroup>
    {
        Task<IEnumerable<FormGroup>> GetAllWithStudentsAndSubjectsAsync();
        Task<FormGroup?> GetByIdWithStudentsAndSubjectsAsync(int id);
    }


}

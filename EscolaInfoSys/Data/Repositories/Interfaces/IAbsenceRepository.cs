using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IAbsenceRepository
    {
        Task<IEnumerable<Absence>> GetAllAsync();
        Task<Absence?> GetByIdAsync(int id);
        Task AddAsync(Absence absence);
        Task UpdateAsync(Absence absence);
        Task DeleteAsync(Absence absence);
        Task<bool> ExistsAsync(int id);

        Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId);
    }
}

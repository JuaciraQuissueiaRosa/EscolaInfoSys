using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);

        Task<Student?> GetFullByIdAsync(int id); // Inclui Absences, Marks e FormGroup
        Task<Student?> GetWithFormGroupAsync(int id); // Inclui apenas FormGroup

        Task<IEnumerable<StudentExclusion>> GetExclusionsAsync(int studentId);
        Task DeleteAsync(Student student);
        Task<bool> ExistsAsync(int id);
    }
}

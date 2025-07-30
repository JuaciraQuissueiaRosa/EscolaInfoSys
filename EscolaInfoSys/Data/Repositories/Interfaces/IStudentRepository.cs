using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<Student?> GetWithFormGroupAsync(int id);
        Task<Student?> GetFullByIdAsync(int id);

        Task<IEnumerable<Student>> GetWithFormGroupAsync();
        Task<IEnumerable<StudentExclusion>> GetExclusionsAsync(int studentId);

        Task<Student?> GetByApplicationUserIdAsync(string applicationUserId);

        Task<Student?> GetByUserIdAsync(string userId);

        Task UpdateSelectedFieldsAsync(Student student);

        Task<bool> IsStudentExcludedFromAnySubjectAsync(int studentId);

     





    }

}

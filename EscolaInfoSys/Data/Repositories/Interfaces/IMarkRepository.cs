using EscolaInfoSys.Models;
using EscolaInfoSys.Models.Dtos;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IMarkRepository : IGenericRepository<Mark>
    {
        Task<IEnumerable<Mark>> GetValidMarksAsync(IEnumerable<int> excludedStudentIds, IEnumerable<int> excludedSubjectIds);

        Task UpdateIsPassedStatusAsync();

        Task UpdateStudentSubjectPassStatusAsync(int studentId, int subjectId);

        Task<List<StudentSubjectAverageDto>> GetStudentSubjectAveragesByStaffAsync(int staffId);

        Task<List<StudentSubjectAverageDto>> GetStudentSubjectAveragesAsync(int studentId);




    }

}

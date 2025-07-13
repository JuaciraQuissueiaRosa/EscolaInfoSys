using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IAbsenceRepository : IGenericRepository<Absence>
    {
        Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId);
    }
}

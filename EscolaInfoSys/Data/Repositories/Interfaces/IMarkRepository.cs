using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IMarkRepository : IGenericRepository<Mark>
    {
        Task<IEnumerable<Mark>> GetValidMarksAsync(IEnumerable<int> excludedStudentIds, IEnumerable<int> excludedSubjectIds);

    }

}

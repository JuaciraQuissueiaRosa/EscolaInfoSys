using EscolaInfoSys.Models;
using EscolaInfoSys.Models.ViewModels;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IAbsenceRepository : IGenericRepository<Absence>
    {
        Task<int> CountByStudentAndSubjectAsync(int studentId, int subjectId);


        Task<Absence?> GetByIdWithStudentAndSubjectAsync(int id);

        Task<Absence?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<Absence>> GetAllWithStudentAndSubjectAsync();

        Task<IEnumerable<Absence>> GetByStudentIdAsync(int studentId);

        Task<(List<Absence> Absences, double Percentage, double Limit)> GetAbsencesWithStatsAsync(int studentId);

        Task<IEnumerable<Absence>> GetAbsencesByStudentIdAsync(int studentId);

        Task<IEnumerable<AbsenceViewModel>> GetAbsenceViewModelsByStudentIdAsync(int studentId);

        Task<int> CountUnjustifiedAbsencesByStudentAndSubjectAsync(int studentId, int subjectId);

        Task UpdateFromViewModelAsync(AbsenceEditViewModel model);

     
        Task<AbsenceEditViewModel?> GetAbsenceEditViewModelAsync(int id);











    }
}

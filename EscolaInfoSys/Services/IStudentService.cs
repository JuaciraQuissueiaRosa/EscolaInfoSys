using EscolaInfoSys.Models.Dtos;
using EscolaInfoSys.Models.ViewModels;

namespace EscolaInfoSys.Services
{
    public interface IStudentService
    {
        Task<(List<AbsenceViewModel> Absences, bool IsExcluded)> GetStudentAbsencesAndExclusionAsync(string userId);

        Task<(List<StudentAbsenceStatusDto> AbsencesStatus, bool IsExcluded)> GetStudentAbsencesAndExclusionStatusAsync(string userId);

    }

}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStaffMemberRepository : IGenericRepository<StaffMember>
    {
        Task<StaffMember?> GetByApplicationUserIdAsync(string applicationUserId);
        Task<IEnumerable<StaffMember>> GetAllWithUserAsync();
        Task<StaffMember?> GetByIdWithUserAsync(int id);
    }

}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface IStaffMemberRepository
    {
        Task<IEnumerable<StaffMember>> GetAllAsync();
        Task<StaffMember?> GetByIdAsync(int id);
        Task AddAsync(StaffMember staff);
        Task UpdateAsync(StaffMember staff);
        Task DeleteAsync(StaffMember staff);
        Task<bool> ExistsAsync(int id);

        Task<StaffMember?> GetByApplicationUserIdAsync(string applicationUserId);

    }
}

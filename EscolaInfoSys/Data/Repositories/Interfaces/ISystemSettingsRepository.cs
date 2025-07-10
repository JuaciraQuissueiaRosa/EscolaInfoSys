using EscolaInfoSys.Models;

namespace EscolaInfoSys.Data.Repositories.Interfaces
{
    public interface ISystemSettingsRepository
    {
        Task<SystemSettings> GetSettingsAsync();
        Task SaveAsync();
        Task UpdateAsync(SystemSettings settings);
    }

}

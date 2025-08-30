namespace EscolaInfoSysApi.Hubs
{
    public interface INotifier
    {
        Task GradeAddedAsync(string userId, object payload);
        Task StatusChangedAsync(string userId, object payload);
    }
}

using Microsoft.AspNetCore.SignalR;

namespace EscolaInfoSysApi.Hubs
{
    public class SignalRNotifier : INotifier
    {
        private readonly IHubContext<NotificationHub> _hub;
        public SignalRNotifier(IHubContext<NotificationHub> hub) => _hub = hub;

        public Task GradeAddedAsync(string userId, object payload) =>
            _hub.Clients.Group($"user:{userId}").SendAsync("gradeAdded", payload);

        public Task StatusChangedAsync(string userId, object payload) =>
            _hub.Clients.Group($"user:{userId}").SendAsync("statusChanged", payload);
    }
}

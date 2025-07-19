using Microsoft.AspNetCore.SignalR;

namespace EscolaInfoSys.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message, string type)
        {
            await Clients.All.SendAsync("ReceiveNotification", message, type);
        }


        public override async Task OnConnectedAsync()
        {
            var role = Context.GetHttpContext()?.User?.FindFirst("role")?.Value;
            if (!string.IsNullOrEmpty(role))
                await Groups.AddToGroupAsync(Context.ConnectionId, role);

            await base.OnConnectedAsync();
        }
    }
}

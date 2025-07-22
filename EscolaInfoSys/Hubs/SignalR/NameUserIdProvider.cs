using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EscolaInfoSys.Hubs.SignalR
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Retorna o ID do usuário logado (UserId do Identity)
            return connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

}

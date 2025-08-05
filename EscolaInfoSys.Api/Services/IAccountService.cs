using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EscolaInfoSys.Api.Services
{
    public interface IAccountService
    {
        Task<string?> AuthenticateAsync(string email, string password);
    }

  
}

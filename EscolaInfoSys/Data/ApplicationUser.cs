using Microsoft.AspNetCore.Identity;

namespace EscolaInfoSys.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? ProfilePhoto { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace EscolaInfoSys.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfilePhoto { get; set; }
    }
}

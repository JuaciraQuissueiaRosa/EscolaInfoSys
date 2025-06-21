using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}

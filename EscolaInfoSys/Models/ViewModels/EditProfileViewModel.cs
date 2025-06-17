using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public string? CurrentPhoto { get; set; }

        [Display(Name = "New Profile Photo")]
        public IFormFile? NewPhoto { get; set; }
    }
}

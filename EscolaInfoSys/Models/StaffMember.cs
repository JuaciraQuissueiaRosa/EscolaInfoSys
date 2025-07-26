using EscolaInfoSys.Data;
using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class StaffMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // FK para ApplicationUser
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }


        public ICollection<Mark>? Marks { get; set; }
    }
}

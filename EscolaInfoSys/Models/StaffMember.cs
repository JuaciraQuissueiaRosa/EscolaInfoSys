using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class StaffMember
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public ICollection<Mark> Marks { get; set; }
    }
}

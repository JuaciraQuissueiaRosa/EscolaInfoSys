using EscolaInfoSys.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Student Number")]
        public string PupilNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Profile Photo")]
        public string? ProfilePhoto { get; set; }

        // FK to Class
        [Display(Name = "Document Photo")]
        public string? DocumentPhoto { get; set; }

        public int FormGroupId { get; set; }

        [ValidateNever]
        public FormGroup FormGroup { get; set; }

        public ICollection<Mark>? Marks { get; set; }
        public ICollection<Absence>? Absences { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}

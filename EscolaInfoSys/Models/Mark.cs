using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class Mark
    {
        public int Id { get; set; }

        [Required]
        [Range(0, 20, ErrorMessage = "The mark must be between 0 and 20.")]
        public float Value { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Student is required")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [Display(Name = "Evaluation Type")]
        [Required]
        public EvaluationType? EvaluationType { get; set; }

        public bool? IsPassed { get; set; }

        [Required(ErrorMessage = "Staff Member is required")]
        public int? StaffMemberId { get; set; }
        public StaffMember? StaffMember { get; set; }
    }
}

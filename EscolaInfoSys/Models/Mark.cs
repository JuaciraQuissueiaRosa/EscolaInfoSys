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

        [Display(Name = "Student")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [Display(Name = "Subject")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        [Display(Name = "Evaluation Type")]
        public EvaluationType EvaluationType { get; set; }

        [Display(Name = "StaffMember")]
        public int? StaffMemberId { get; set; }
        public StaffMember? StaffMember { get; set; }
    }
}

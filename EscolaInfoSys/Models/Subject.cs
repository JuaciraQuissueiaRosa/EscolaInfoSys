using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class Subject
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Course field is required.")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Required]
        [Display(Name = "Total Lessons")]
        [Range(1, int.MaxValue, ErrorMessage = "Total lessons must be at least 1.")]
        public int TotalLessons { get; set; }

        public ICollection<Mark>? Marks { get; set; }
        public ICollection<Absence>? Absences { get; set; }

        public int? FormGroupId { get; set; }
        public FormGroup? FormGroup { get; set; }
    }

}

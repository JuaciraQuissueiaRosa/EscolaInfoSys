using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class FormGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public ICollection<Student>? Students { get; set; } = new List<Student>();
        public ICollection<Subject>? Subjects { get; set; }

        // Nova propriedade: curso ao qual a turma pertence

        [Display(Name = "Course")]
        public int? CourseId { get; set; }   
        public Course? Course { get; set; }
    }
}

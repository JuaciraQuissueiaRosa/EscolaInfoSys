using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class FormGroup
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Student> Students { get; set; }
        public ICollection<Subject> Subjects { get; set; }
    }
}

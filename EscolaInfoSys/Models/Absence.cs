using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class Absence
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [Display(Name = "Student")]

        public int ? StudentId { get; set; }
        public Student? Student { get; set; }


        [Display(Name = "Subject")]
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public bool Justified { get; set; }
    }
}

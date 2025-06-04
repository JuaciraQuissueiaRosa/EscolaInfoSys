using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public class Absence
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public bool Justified { get; set; }
    }
}

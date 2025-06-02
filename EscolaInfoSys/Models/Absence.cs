namespace EscolaInfoSys.Models
{
    public class Absence
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public bool Justified { get; set; }
    }
}

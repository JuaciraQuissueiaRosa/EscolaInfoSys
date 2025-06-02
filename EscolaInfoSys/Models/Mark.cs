namespace EscolaInfoSys.Models
{
    public class Mark
    {
        public int Id { get; set; }
        public float Value { get; set; }
        public DateTime Date { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}

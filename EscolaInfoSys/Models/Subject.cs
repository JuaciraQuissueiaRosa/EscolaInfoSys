namespace EscolaInfoSys.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int TotalLessons { get; set; }

        public ICollection<Mark>? Marks { get; set; }
        public ICollection<Absence>? Absences { get; set; }
    }
}

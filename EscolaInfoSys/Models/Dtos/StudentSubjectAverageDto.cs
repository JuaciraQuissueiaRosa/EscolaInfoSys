namespace EscolaInfoSys.Models.Dtos
{
    public class StudentSubjectAverageDto
    {
        public int? StudentId { get; set; }
        public string StudentEmail { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public double Average { get; set; }
        public bool IsPassed { get; set; }
    }

}

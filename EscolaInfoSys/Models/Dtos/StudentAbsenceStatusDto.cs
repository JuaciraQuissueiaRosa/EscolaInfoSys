namespace EscolaInfoSys.Models.Dtos
{
    public class StudentAbsenceStatusDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int AbsencesCount { get; set; }
        public double AbsencePercentage { get; set; }
        public bool IsExcluded { get; set; }
    }
}

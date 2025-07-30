namespace EscolaInfoSys.Models.ViewModels
{
    public class AbsenceViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool Justified { get; set; }
    }
}

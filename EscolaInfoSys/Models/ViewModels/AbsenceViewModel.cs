namespace EscolaInfoSys.Models.ViewModels
{
    public class AbsenceViewModel
    {
        public string SubjectName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool Justified { get; set; }
    }
}

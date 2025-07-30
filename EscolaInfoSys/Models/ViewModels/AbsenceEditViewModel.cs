namespace EscolaInfoSys.Models.ViewModels
{
 
        public class AbsenceEditViewModel
        {
            public int Id { get; set; }             
            public int StudentId { get; set; }
            public int SubjectId { get; set; }
            public DateTime Date { get; set; }
            public bool Justified { get; set; }
        }

    
}

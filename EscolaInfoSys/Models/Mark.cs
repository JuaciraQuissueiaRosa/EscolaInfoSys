using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Evaluation Type")]
        public string? EvaluationType { get; set; }  // Ex: Test, Exam, Assignment

        public int StaffMemberId { get; set; }
        public StaffMember StaffMember { get; set; }
    }
}

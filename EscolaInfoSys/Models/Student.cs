namespace EscolaInfoSys.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string PupilNumber { get; set; }
        public string Email { get; set; }
        public string ProfilePhoto { get; set; }
        public string DocumentPhoto { get; set; }

        public int FormGroupId { get; set; }
        public FormGroup FormGroup { get; set; }

        public ICollection<Mark> Marks { get; set; }
        public ICollection<Absence> Absences { get; set; }
    }
}

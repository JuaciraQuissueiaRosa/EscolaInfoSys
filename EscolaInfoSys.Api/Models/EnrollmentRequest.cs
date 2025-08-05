using EscolaInfoSys.Models;

namespace EscolaInfoSys.Api.Models
{
    public class EnrollmentRequest
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public DateTime RequestDate { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

        public DateTime? ResponseDate { get; set; }
    }
}

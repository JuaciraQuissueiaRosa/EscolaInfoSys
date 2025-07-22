using EscolaInfoSys.Data;

namespace EscolaInfoSys.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsResolved { get; set; }

        public string? AdminResponse { get; set; }

        public int StaffId { get; set; } 
        public StaffMember? StaffMember { get; set; } 
    }
}

namespace EscolaInfoSys.Api.Models
{
    public class UpdateStudentProfileDto
    {
        public string UserName { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }

}

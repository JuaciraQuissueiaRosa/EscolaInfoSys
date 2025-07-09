namespace EscolaInfoSys.Models
{
    public class SystemSettings
    {
        public int Id { get; set; }
        public double MaxAbsencePercentage { get; set; } = 30.0; // Padrão 30%
    }
}

using EscolaInfoSys.Models;

namespace EscolaInfoSys.Services
{
    public class AbsenceStatsResult
    {
        public IEnumerable<Absence> Absences { get; set; }
        public IEnumerable<StudentExclusion> Exclusions { get; set; }
        public Dictionary<string, double> Percentages { get; set; } 
        public Dictionary<int, int> MaxAbsences { get; set; } 
    }
}

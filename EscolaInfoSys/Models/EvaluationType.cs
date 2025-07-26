using System.ComponentModel.DataAnnotations;

namespace EscolaInfoSys.Models
{
    public enum EvaluationType
    {
        [Display(Name = "Written Test")]
        Test,

        [Display(Name = "Homework Assignment")]
        Assignment,

        [Display(Name = "Group Project")]
        Project,

        [Display(Name = "Oral Examination")]
        OralExam
    }
}

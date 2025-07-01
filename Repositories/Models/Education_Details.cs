using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_Education_Details
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_education_id{get;set;}

    public int c_user_id{get;set;}

    public string c_HighestQualification{get;set;}

    public string? c_Degree{get;set;}
    public string? c_Specialization{get;set;}

    public string? c_UniversityName{get;set;}
    public int c_YearOfPassing{get;set;}
    public decimal c_percentage{get;set;}
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_Work_Experience
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_WorkID{get;set;}

    public int c_user_id{get;set;}

    public string c_CompanyName{get;set;}

    public string c_JobTitle{get;set;}

    public string c_JobDesc{get;set;}

    public int c_years_work{get;set;}

    public bool c_CurrentlyWorking{get;set;}
}
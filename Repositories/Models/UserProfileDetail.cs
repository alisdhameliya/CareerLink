using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_UserProfileDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_user_id{get;set;}

    public string c_fullname{get;set;}

    public string c_email{get;set;}

    public string c_mobile_number{get;set;}

    public string c_gender{get;set;}

    public string c_image{get;set;}
    public string c_role{get;set;}

    public int c_education_id{get;set;}
    public t_Education_Details? education_Details{get;set;}

    public int c_workid{get;set;}
    public t_Work_Experience? experience{get;set;}

    public int c_skill_id{get;set;}
    public t_UserSkills skills{get;set;}

    public int c_preferenceid{get;set;}
    public t_JobPreference? preference{get;set;}

    public int c_projectid{get;set;}
    public t_UserProjects? projects{get;set;}

    public int? profile_completion_percentage{get;set;}
}
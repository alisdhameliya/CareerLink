using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_UserProjects
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_ProjectID{get;set;}
    public int c_user_id{get;set;}
    public string c_Project_Title{get;set;}
    public string c_Project_Description{get;set;}
    public string c_TechnologiesUsed{get;set;}
    public string c_ProjectLink{get;set;}
}
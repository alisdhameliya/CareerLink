using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_UserSkills
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_skill_id{get;set;}

    public string c_skill_name{get;set;}

    public int c_user_id{get;set;}

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;

public class t_JobPreference
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_PreferenceID{get;set;}

    public int c_user_id{get;set;}
    public string? c_PreferredRoles{get;set;}

    public string? c_PreferredSalary{get;set;}
    public string? c_PreferredLocations{get;set;}


}
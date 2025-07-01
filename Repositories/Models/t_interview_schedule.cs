using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class t_interview_schedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_interview_id{get;set;}

    public t_user? user{get;set;}
    public int c_user_id{get;set;}

    public string c_interview_date{get;set;}

    public string c_interview_time{get;set;}
    public string? c_meeting_url{get;set;}
    public int c_company_id{get;set;}

    
    [NotMapped]
    public string c_email { get; set; }

    [NotMapped]
    public string c_fullName { get; set; }
}
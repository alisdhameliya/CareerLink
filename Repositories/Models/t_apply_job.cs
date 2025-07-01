using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Repositories.Model;
using Repositories.Models;

public class t_apply_job
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_application_id{get;set;}

    public Job_Post? job_Post{get;set;}
    public int c_job_id{get;set;}

    public t_user? user{get;set;}
    public int c_uid{get;set;}

    public t_Education_Details? education_Details{get;set;}
    public t_Work_Experience? work_Experience{get;set;}
    public t_UserResume? userResume{get;set;}
    public t_Company? company{get;set;}

    public string? c_apply_date{get;set;}

    public string? c_status{get;set;}
}

public class FieldCheckResult
{
    public bool HasEducation { get; set; }
    public bool HasExperience { get; set; }
    public bool HasResume { get; set; }
}

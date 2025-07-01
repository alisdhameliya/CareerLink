using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

public class t_UserResume
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_ResumeID{get;set;}
    public int c_user_id{get;set;}
    public string? c_ResumeFilePath{get;set;}
    public IFormFile? c_ResumeFile{get;set;}
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

public class t_User_Certificate
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_CertificationID{get;set;}

    public int c_user_id{get;set;}

    public string? c_CertificateFilePath{get;set;}

    public IFormFile? c_certificate{get;set;}
}
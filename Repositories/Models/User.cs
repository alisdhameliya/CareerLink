using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

public class t_user
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int c_userId { get; set; }

    [JsonPropertyName("Name")]
    public string? c_username { get; set; }

    public string? c_fullName { get; set; }

    [JsonPropertyName("Email")]
    public string? c_email { get; set; }

    public string? c_password { get; set; }

    public string? c_phoneNumber { get; set; }

    public string? c_gender { get; set; }

    public string? c_profileImage { get; set; }

    public IFormFile? c_image { get; set; }

    [JsonPropertyName("Role")]
    public string? c_userRole { get; set; }
}
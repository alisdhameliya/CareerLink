using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repositories.Model
{
    public class t_user1
    {
        [Key]
        public int c_userId { get; set; }

        [Required]
        [JsonPropertyName("Name")]
        public string c_username { get; set; } 

        [Required]
        public string c_fullName { get; set; } 

        [Required, EmailAddress]
        [JsonPropertyName("Email")]
        public string c_email { get; set; }

        [Required]
        public string c_password { get; set; }

        [Required]
        public string c_phoneNumber { get; set; } 

        [Required]
        public string? c_gender { get; set; } 

        public string? c_profileImage { get; set; }

        public IFormFile? ProfilePic { get; set; }

        [Required]
        [JsonPropertyName("Role")]
        public string c_userRole { get; set; }
    }
}
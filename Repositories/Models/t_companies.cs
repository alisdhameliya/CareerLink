using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Repositories.Models
{
    public class t_companies
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_company_id { get; set; }  // Company ID
        
        [Required]
        public string c_company_name { get; set; }  // Company Name
        
        [Required]
        public int c_owner_id { get; set; }  // Owner ID
        
        [Required, EmailAddress]
        public string c_company_email { get; set; }  // Company Email
        
        [Required]
        public string c_company_phone { get; set; }  // Company Phone
        
        [Required]
        public string c_company_address { get; set; }  // Company Address
        
        public string c_company_reg_number { get; set; }  // Company Registration Number
        
        public string c_tax_id_number { get; set; }  // Tax ID Number
        
        public string c_industry { get; set; }  // Industry
        
        public string? c_website { get; set; }  // Website
        
        // public bool c_verified_status { get; set; }  // Verified Status
        
        public string[]? c_legal_documents { get; set; }  // Array of Legal Document File Names
        
        public string? c_company_logo { get; set; }  // **Company Logo Path (Stored in DB)**

        [NotMapped]  // **Prevents storing in DB**
        [JsonIgnore] // **Avoids serialization in API responses**
        public List<IFormFile>? LegalDocuments { get; set; }  // **For Uploading Legal Docs**
        
        [NotMapped]
        [JsonIgnore]
        public IFormFile? CompanyLogo { get; set; }  // **For Uploading Company Logo**
    }
}
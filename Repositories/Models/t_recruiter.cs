using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Model
{
    public class t_recruiter
    {
        [Key]
        [Column("c_company_id")]
        public int c_company_id { get; set; }

        [Required]
        [Column("c_company_name")]
        [MaxLength(255)]
        public string c_company_name { get; set; }

        [Required]
        [Column("c_owner_id")]
        public int c_owner_id { get; set; } 

        [Required]
        [Column("c_company_email")]
        [MaxLength(255)]
        [EmailAddress]
        public string c_company_email { get; set; }

        [Required]
        [Column("c_company_phone")]
        [MaxLength(20)]
        public string c_company_phone { get; set; }

        [Required]
        [Column("c_company_address")]
        public string c_company_address { get; set; }

        [Required]
        [Column("c_company_reg_number")]
        [MaxLength(100)]
        public string c_company_reg_number { get; set; }

        [Required]
        [Column("c_tax_id_number")]
        [MaxLength(100)]
        public string c_tax_id_number { get; set; }

        [Required]
        [Column("c_industry")]
        [MaxLength(100)]
        public string c_industry { get; set; }

        [Column("c_website")]
        [MaxLength(255)]
        public string? c_website { get; set; }

        [Column("c_verified_status")]
        public bool? c_verified_status { get; set; } = false;

        [Column("c_legal_documents")]
        public string[]? c_legal_documents { get; set; }
        [Column("c_company_logo")]
        public string? c_company_logo { get; set; }
    }
}
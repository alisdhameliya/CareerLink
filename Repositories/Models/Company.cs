using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Model
{
    public class t_Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_company_id { get; set; }

        public string c_company_name{get;set;}

        public string c_company_logo{get;set;}
        public string c_company_email{get;set;}
        public string c_company_phone_number{get;set;}
        public string c_company_address{get;set;}
        public string c_company_industry{get;set;}
        public string c_company_website{get;set;}
    }
}
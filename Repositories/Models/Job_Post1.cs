using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Repositories.Models
{
    public class Job_Post1
    {
         public int? c_job_id { get; set; }

        [Required]
        public string c_job_title { get; set; }
        public string? c_post_date { get; set; }


        [Required]
        public string c_job_desc { get; set; }

        [Required]
        public string c_job_location { get; set; }

        [Required]
        public string c_job_type { get; set; }

        [Required]
        public int c_job_experience { get; set; }

        [Required]
        public string c_salary_range { get; set; }

        [Required]
        public int c_vacancy { get; set; }

        [Required]
        public int c_dept_id { get; set; }

        [Required]
        public string c_qualification_title { get; set; }

        [Required]
        public string c_skills { get; set; }

        [Required]
        public int c_company_id { get; set; }
        public string c_company_name { get; set; }
        public string c_dept_name { get; set; }
        public string c_company_logo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Model
{
    [Table("t_job_post")]
    public class Job_Post
    {
        public int? c_job_id { get; set; }

        [Required]
        public string c_job_title { get; set; }

        [Required]
        public string c_job_desc { get; set; }

        public string? c_post_date { get; set; }

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
        public t_department? department { get; set; }

        [Required]
        public string c_qualification_title { get; set; }

        [Required]
        public string c_skills { get; set; }

        [Required]
        public int c_company_id { get; set; }

        // [Required]
        public string? c_company_name { get; set; }
        public t_Company? company { get; set; }
    }
}
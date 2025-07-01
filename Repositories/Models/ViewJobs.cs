using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace Repositories.Model
{
    public class t_ViewJobs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_job_id { get; set; }

        public string c_job_title { get; set; }

        public string c_job_desc { get; set; }

        public string? c_post_date { get; set; }

        public string c_job_location { get; set; }

        public string c_job_type { get; set; }

        public int c_job_experience { get; set; }

        public string c_salary_range { get; set; }

        public int c_vacancy { get; set; }

        public int c_dept_id { get; set; }
        public t_department? department { get; set; }

        public string c_qualification_title { get; set; }

        public string c_skills { get; set; }

        public t_Company? company{get;set;}
        public int c_company_id{get;set;}
    }
}
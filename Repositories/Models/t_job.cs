using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    [Table("t_job_post")]
    public class t_job
    {
        public int? c_job_id { get; set; }
        public string c_job_title { get; set; }
        
        // [Column(TypeName = "timestamp without time zone")]
        // public string? c_post_date { get; set; }
        
        public string c_job_location { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Model
{
    public class vm_Login
    {
        [Required, EmailAddress]
        public string c_email { get; set; }

        [Required]
        public string c_password { get; set; }

        [Required]
        public string c_userRole { get; set; }
    }
}
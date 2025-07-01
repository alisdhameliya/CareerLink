using System;

namespace Repositories.Models
{
    public class RegistrationStats
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int NewCompanies { get; set; }
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ICandidateInterface
    {
        public Task<List<Repositories.Models.Job_Post1>> GetJobs();
        
    }
}
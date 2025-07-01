using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.Interface
{
    public interface IPostedJobInterface
    {
        Task<List<Job_Post>> GetJobDetails(); //recruiter
        Task<List<t_ViewJobs>> GetAllJobs(); //candidate

        Task<Job_Post> GetJobDescription(int id);
    }
}
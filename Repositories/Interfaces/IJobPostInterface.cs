using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.Interfaces
{
    public interface IJobPostInterface
    {
        Task<int> CreateJob(Job_Post job);
        
        Task<int> EditJob(int id, Job_Post job);

        Task<int> DeleteJob(int id);

        Task<List<t_department>> getAllDepartments();

        Task<List<t_skills>> getAllSkills();



        Task<List<Job_Post>> GetJobDetails(int id); //recruiter
        Task<List<t_ViewJobs>> GetAllJobs(); //candidate
        Task<Job_Post> GetOneJob(int id);

    }
}
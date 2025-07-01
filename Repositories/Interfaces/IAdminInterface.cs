using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface IAdminInterface
    {

         public Task<List<t_user>> GetUsers();


         public Task<bool> DeleteUser(int id);

         public Task<int> GetUsersCount();

         public Task<int> GetRecruitersCount();

        public Task<List<t_recruiter>> GetRecruiters();

        Task<bool> UpdateRecruiterStatus(int companyId, bool approved = true);

        Task<bool> UpdateRecruiterstatusReject(int companyId,string reason, bool approved = false);

        Task<bool> BulkUpdateRecruiterStatus(List<int> companyIds, bool approved = true);

        Task<bool> BulkUpdateRecruiterStatusReject(List<int> companyIds,string reason, bool approved = false);
        Task<bool> DeleteRecruiter(int companyId);
        Task<List<RegistrationStats>> GetRegistrationStats(DateTime startDate, DateTime endDate);
        Task<UserDistribution> GetUserDistribution();
        public Task<int> GetJobPostCount();
        Task<List<t_recruiter>> PendingApproval();
        public Task<List<t_job>> GetJobs();
    }
}
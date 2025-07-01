using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Model;
using Repositories.Models;

namespace Repositories.Interfaces
{
    public interface ICompanyInterface
    {
        Task<t_user> GetRecruiterById(int id);
        Task<int> RegisterCompany(t_companies company);
        Task<int> GetCompanyId(int id);
        Task<Job_Post> GetCompanyName(int id);
        Task<List<t_companies>> GetMandatoryFields();
        Task<int> GetCompanyStatus(int id);
    }
}
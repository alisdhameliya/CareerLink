using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Model;

namespace Repositories.Interfaces
{
    public interface IAuthInterface
    {
        Task<int> Register(t_user1 user);
        Task<t_user> Login(vm_Login login);
        Task<bool> ChangePassword(string userEmail, string NewPassword);
        Task<int> GetUserEmail (string email);
    }
}
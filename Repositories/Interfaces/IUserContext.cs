using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserContext
    {
        int? GetCurrentUserId();
        string? GetCurrentUserEmail();
        bool IsAuthenticated();
    }
}
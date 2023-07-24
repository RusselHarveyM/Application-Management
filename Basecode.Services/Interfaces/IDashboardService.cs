using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IDashboardService
    {
        Task UpdateStatus(Application application, User user, string status);
        List<Application> GetShorlistedApplicatons(string type);
        Application GetApplicationById(Guid id);
    }
}

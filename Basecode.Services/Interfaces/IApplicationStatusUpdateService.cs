using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface IApplicationStatusUpdateService
    {
        void UpdateApplicationStatus(Application application, User user, string choice, string newStatus);
    }
}

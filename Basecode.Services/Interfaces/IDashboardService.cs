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
        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        void UpdateStatus(Application application, User user, string status);
        /// <summary>
        /// Gets the shorlisted applicatons.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        List<Application> GetShorlistedApplicatons(string type, int jobId);
        /// <summary>
        /// Gets the application by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Application GetApplicationById(Guid id);
    }
}

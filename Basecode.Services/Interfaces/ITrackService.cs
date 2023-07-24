using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITrackService
    {
        /// <summary>
        /// Updates the track status email.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="appId">The application identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="newStatus">The new status.</param>
        /// <param name="mailType">Type of the mail.</param>
        /// <returns></returns>
        Task UpdateTrackStatusEmail(Applicant applicant, Guid appId, int userId, string newStatus, string mailType);

        /// <summary>
        /// Statuses the notification.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="user">The user.</param>
        /// <param name="newStatus">The new status.</param>
        /// <returns></returns>
        Task StatusNotification(Applicant applicant, User user, string newStatus);

        Task RegretNotification(Applicant applicant, string job);
    }
}

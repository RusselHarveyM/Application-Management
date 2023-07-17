using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Interfaces
{
    public interface ITrackService
    {
        Task UpdateTrackStatusEmail(Applicant applicant, Guid appId, int userId, string newStatus, string mailType);

        Task StatusNotification(Applicant applicant, User user, string newStatus);
    }
}

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
        Task UpdateTrackStatus(Applicant applicant, int userId, string newStatus, string mailType);
    }
}

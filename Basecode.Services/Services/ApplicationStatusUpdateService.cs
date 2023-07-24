using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class ApplicationStatusUpdateService : IApplicationStatusUpdateService
    {
        private readonly ITrackService _trackService;

        public ApplicationStatusUpdateService(ITrackService trackService)
        {
            _trackService = trackService;
        }

        public void UpdateApplicationStatus(Application application, User user, string choice, string newStatus)
        {
            _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, newStatus);
        }
    }
}

using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ITrackService _trackService;
        private readonly IApplicationService _applicationService;

        public DashboardService(ITrackService trackService, IApplicationService applicationService)
        {
            _trackService = trackService;
            _applicationService = applicationService;
        }

        public List<Application> GetShorlistedApplicatons(string type)
        {
            return _applicationService.GetShorlistedApplicatons(type);
        }

        public Application GetApplicationById(Guid id)
        {
            return _applicationService.GetApplicationById(id);
        }

        public async Task UpdateStatus(Application application, User user, string status)
        {
            var result = await _trackService.UpdateApplicationStatus(application, user, status, null);
            if(result != null)
            {
                _applicationService.Update(result);
            }
        }
    }
}

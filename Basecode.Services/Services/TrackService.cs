using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{
    public class TrackService : ITrackService
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;

        public TrackService(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        public async Task UpdateTrackStatusEmail(Applicant applicant,Guid appId, int userId, string newStatus, string mailType)
        {
            
            if (applicant.Id>= 0 && userId >= -1)
            {
                var user = _userService.GetById(userId);

                switch (mailType)
                {
                    case "GUID":
                        //for GUID, method for change.
                        await _emailService.SendNotifyEmail(applicant, newStatus);
                        break;
                    case "Approval":
                        await _emailService.SendApprovalEmail(user, applicant, appId, newStatus);
                        break;
                    case "Rejected":
                        await _emailService.SendRejectedEmail(applicant, newStatus);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task StatusNotification(Applicant applicant, User user, string newStatus)
        {
            if (applicant.Id >= 0)
            {
                //Notify HR and Applicant for every status change.
                await _emailService.SendStatusNotification(user, applicant, newStatus);
            }
        }

    }
}

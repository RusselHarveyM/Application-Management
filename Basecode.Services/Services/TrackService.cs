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

        public async Task UpdateTrackStatus(Applicant applicant, int userId, string newStatus, string mailType)
        {
            if (applicant.Id>= 0 && userId >= -1)
            {
                var user = _userService.GetById(userId);

                switch (mailType)
                {
                    case "Notify01":
                        //for GUID, method for change.
                        await _emailService.SendNotifyEmail(applicant, newStatus);
                        break;
                    case "Notify02":
                        await _emailService.SendNotifyEmail(applicant, newStatus);
                        await _emailService.SendNotifyHREmail(applicant, newStatus);
                        break;
                    case "Approval":
                        await _emailService.SendApprovalEmail(user, applicant);
                        break;
                    case "Rejected":
                        await _emailService.SendRejectedEmail(applicant, newStatus);
                        break;
                    default:
                        break;
                }
            }
        }

       
    }
}

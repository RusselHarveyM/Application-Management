using Basecode.Data.Models;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IInterviewService
    {
        LogContent AddInterview(UserSchedule schedule);
    }
}

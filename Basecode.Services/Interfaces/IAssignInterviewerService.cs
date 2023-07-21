using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using System.Collections.Generic;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Interfaces
{
    public interface IAssignInterviewerService
    {
        List<AssignInterviewerViewModel> GetJobs();
        void Create(string JobPosition, string Email);

        AssignInterviewerViewModel GetById(int id);
    }
}

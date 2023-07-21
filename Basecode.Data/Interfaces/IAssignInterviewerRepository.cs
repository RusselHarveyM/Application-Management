using Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface IAssignInterviewerRepository
    {
        IQueryable<AssignInterviewer> GetAll();
        void AddAssign(AssignInterviewer assignInterviewer);

        AssignInterviewer GetJobAssignById(int id);
    }
}

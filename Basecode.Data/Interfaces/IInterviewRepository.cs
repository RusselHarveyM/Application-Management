using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Interfaces
{
    public interface IInterviewRepository
    {
        IQueryable<Interview> GetAll();
        void AddInterview(Interview interview);
        Interview GetInterviewById(int id);
        void UpdateInterview(Interview interview);
        void DeleteInterview(Interview interview);
    }
}

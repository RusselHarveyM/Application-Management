using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class AssignViewModel
    {
        public int UserId { get; set; }
        public List<JobOpeningViewModel> JobOpenings { get; set; }
    }
}

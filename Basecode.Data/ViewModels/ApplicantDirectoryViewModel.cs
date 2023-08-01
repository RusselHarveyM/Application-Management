using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class ApplicantDirectoryViewModel
    {
        public List<Applicant> Applicants { get; set; }
        public ShortListedViewModel? Shortlists { get; set; }
        
        public List<JobOpeningViewModel>? JobOpenings { get; set; }
    }
}

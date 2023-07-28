using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class ApplicantDirectoryViewModel
    {
        public List<System.ValueTuple<string, string, string>> Applicants { get; set; }
        public ShortListedViewModel Shortlists { get; set; }
    }
}

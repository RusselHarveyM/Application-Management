using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class ShortListedViewModel
    {
        public List<Application> HRShortlisted { get;set; }
        public List<Application> TechShortlisted { get;set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class JobOpening
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string EmploymentType { get; set; }
        public string WorkSetup { get; set; }
        public string Location { get; set; }
        public string CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }

        public ICollection<Qualification> Qualifications { get; } = new List<Qualification>();
        public ICollection<Responsibility> Responsibilities { get; } = new List<Responsibility>();
        public ICollection<Application> Applications { get; } = new List<Application>();
        public ICollection<User> Users { get; } = new List<User>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class Application
    {
        public Guid Id { get; set; }
        public int JobOpeningId { get; set; }
        public int ApplicantId { get; set; }
        public string Status { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime UpdateTime { get; set; }

        public JobOpening JobOpening { get; set; } = null!;
        public Applicant Applicant { get; set; } = null!;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class CurrentHire
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public string Firstname { get; set; }
        public string? Middlename { get; set; }
        public string Lastname { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Status { get; set; } = "hired";

        public User User { get; set; } = null!;
        public Application Application { get; set; } = null!;
    }
}

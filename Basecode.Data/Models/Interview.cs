using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class Interview
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public string? TeamsLink { get; set; }
        public string? Type { get; set; }
        public string? Result { get; set; }

        public Application Application { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}

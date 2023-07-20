using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.Models
{
    public class Examination
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? Date { get; set; }
        public string? TeamsLink { get; set; }
        public int? Score { get; set; }
        public string? Result { get; set; }

        [ForeignKey("Application")]
        public Guid ApplicationId { get; set; }

        public virtual Application Application { get; set; }
    }
}

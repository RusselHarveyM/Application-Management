using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class Responsibility
    {
        public int Id { get; set; }

        public int JobOpeningId { get; set; }

        [Required(ErrorMessage = "The Description is required.")]
        public string Description { get; set; }
    }
}

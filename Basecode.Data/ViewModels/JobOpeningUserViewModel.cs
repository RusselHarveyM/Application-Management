using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class JobOpeningUserViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the job opening.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the applcation.
        /// </summary>
        [Required(ErrorMessage = "Please enter a valid JobOpeningId ID.")]
        [RegularExpression(@"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$", ErrorMessage = "Please enter a valid Application ID.")]
        public int JobOpeningId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the applcation.
        /// </summary>
        [Required(ErrorMessage = "Please enter a valid User ID.")]
        [RegularExpression(@"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$", ErrorMessage = "Please enter a valid Application ID.")]
        public int UserId { get; set; }
    }
}

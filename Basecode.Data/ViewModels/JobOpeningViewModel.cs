using Basecode.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels
{
    /// <summary>
    /// View model for a job opening.
    /// </summary>
    public class JobOpeningViewModel
    {
        /// <summary>
        /// Gets or sets the ID of the job opening.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the job opening.
        /// </summary>
        [Required(ErrorMessage = "The title is required.")]
        [MaxLength(50, ErrorMessage = "Maximum length for the title is 50 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the employment type of the job opening.
        /// </summary>
        [Required(ErrorMessage = "The Employment type is required.")]
        [Display(Name = "Employment Type")]
        public string EmploymentType { get; set; }

        /// <summary>
        /// Gets or sets the work setup of the job opening.
        /// </summary>
        [Required(ErrorMessage = "The Work Setup is required.")]
        [Display(Name = "Work Setup")]
        public string WorkSetup { get; set; }

        /// <summary>
        /// Gets or sets the location of the job opening.
        /// </summary>
        [Required(ErrorMessage = "The Location is required.")]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the qualifications of the job opening.
        /// </summary>
        [Required(ErrorMessage = "At least one qualification is required.")]
        public List<Qualification> Qualifications { get; set; }

        /// <summary>
        /// Gets or sets the responsibilities of the job opening.
        /// </summary>
        [Required(ErrorMessage = "At least one responsibility is required.")]
        public List<Responsibility> Responsibilities { get; set; }

        /// <summary>
        /// Collection navigation for related Applications.
        /// </summary>
        public ICollection<Application> Applications { get; } = new List<Application>();

    }
}

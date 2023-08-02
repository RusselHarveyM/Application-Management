using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels;

/// <summary>
///     View model for application tracker.
/// </summary>
public class ApplicationViewModel
{
    /// <summary>
    ///     Gets or sets the ID of the applcation.
    /// </summary>
    [Required(ErrorMessage = "Please enter a valid Application ID.")]
    [RegularExpression(@"^[A-Fa-f0-9]{8}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{4}-[A-Fa-f0-9]{12}$",
        ErrorMessage = "Please enter a valid Application ID.")]
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the job opening title.
    /// </summary>
    public string JobOpeningTitle { get; set; }

    /// <summary>
    ///     Gets or sets the applicant name.
    /// </summary>
    public string ApplicantName { get; set; }

    /// <summary>
    ///     Gets or sets the application status.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    ///     Gets or sets the update time of the application.
    /// </summary>
    public DateTime UpdateTime { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels;

public class BackgroundCheckFormViewModel
{
    public int CharacterReferenceId { get; set; }
    public int UserId { get; set; }

    [Required(ErrorMessage = "Firstname is required")]
    public string Firstname { get; set; }

    [Required(ErrorMessage = "Lastname is required")]
    public string Lastname { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Not a valid email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "PhoneNumber is required")]
    [RegularExpression(@"^09\d{9}$",
        ErrorMessage =
            "Invalid phone number format. The phone number must start with '09' and have 11 digits in total.")]
    [StringLength(11, ErrorMessage = "Phone must be 11 numbers long.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Relationship is required")]
    public string Relationship { get; set; }

    [Required(ErrorMessage = "The textarea must not be left blank.")]
    public string? Q1 { get; set; }

    [Required(ErrorMessage = "The textarea must not be left blank.")]
    public string? Q2 { get; set; }

    [Required(ErrorMessage = "The textarea must not be left blank.")]
    public string? Q3 { get; set; }

    [Required(ErrorMessage = "The textarea must not be left blank.")]
    public string? Q4 { get; set; }
}
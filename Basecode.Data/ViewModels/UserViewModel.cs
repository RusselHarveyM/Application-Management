using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels;

/// <summary>
///     Provides a customized representation of user information.
/// </summary>
public class UserViewModel
{
    /// <summary>
    ///     Represents the unique identifier of a user.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    ///     Represents the full name of a user.
    /// </summary>
    [Required]
    [DisplayName("Full Name")]
    [StringLength(50)]
    [RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿ ,.'-]+$", ErrorMessage = "Special characters are not allowed.")]
    public string Fullname { get; set; }

    /// <summary>
    ///     Represents the username associated with a user.
    /// </summary>
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Special characters and spaces are not allowed.")]
    [StringLength(50)]
    public string Username { get; set; }

    /// <summary>
    ///     Represents the email address of a user.
    /// </summary>
    [Required]
    [DisplayName("Email Address")]
    [EmailAddress]
    [StringLength(50)]
    public string Email { get; set; }

    /// <summary>
    ///     Represents the password associated with a user.
    /// </summary>
    [Required(ErrorMessage = "The password is required.")]
    [MaxLength(20, ErrorMessage = "Maximum length for the password is 20 characters.")]
    [RegularExpression("^(?=.*[^a-zA-Z0-9])(?=.*[0-9])(?=.*[A-Z]).{8,}$",
        ErrorMessage =
            "Passwords must have at least one non-alphanumeric character, one digit, and one uppercase letter, and should be at least 8 characters long.")]
    public string Password { get; set; }

    /// <summary>
    ///     Represents the role or position of a user in the system.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Role { get; set; }
}
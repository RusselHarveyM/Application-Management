using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels;

public class LoginViewModel
{
    /// <summary>
    ///     Gets or sets the email.
    /// </summary>
    /// <value>
    ///     The email.
    /// </value>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    /// <summary>
    ///     Gets or sets the password.
    /// </summary>
    /// <value>
    ///     The password.
    /// </value>
    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(20, ErrorMessage = "Maximum length for the password is 20 characters.")]
    public string Password { get; set; }
}
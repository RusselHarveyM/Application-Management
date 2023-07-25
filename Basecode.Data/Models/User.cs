using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    /// <summary>
    /// Represents a user entity with properties that store information.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Represents the unique identifier of a user.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Represents the full name of a user.
        /// </summary>
        [Required]
        [DisplayName("Full Name")]
        [StringLength(50)]
        [RegularExpression("^[A-Za-zÀ-ÖØ-öø-ÿ ,.'-]+$", ErrorMessage = "Special characters are not allowed.")]
        public string Fullname { get; set; }

        /// <summary>
        /// Represents the username associated with a user.
        /// </summary>
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Special characters and spaces are not allowed.")]
        [StringLength(50)]
        public string Username { get; set; }

        /// <summary>
        /// Represents the email address of a user.
        /// </summary>
        [Required]
        [DisplayName("Email Address")]
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }

        /// <summary>
        /// Represents the password associated with a user.
        /// </summary>
        [Required(ErrorMessage = "The password is required.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the password is 20 characters.")]
        [RegularExpression(
            @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
            ErrorMessage = "The password must have at least eight characters, one letter, and one number")]
        public string Password { get; set; }

        /// <summary>
        /// Represents the role or position of a user in the system.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Role { get; set; }
    }
}

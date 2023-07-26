﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        /// Gets or sets the ASP identifier.
        /// </summary>
        /// <value>
        /// The ASP identifier.
        /// </value>
        [ForeignKey("IdentityUser")]
        public string AspId { get; set; }

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
        [RegularExpression("^(?=.*[^a-zA-Z0-9])(?=.*[0-9])(?=.*[A-Z]).{8,}$",
        ErrorMessage = "Passwords must have at least one non-alphanumeric character, one digit, and one uppercase letter, and should be at least 8 characters long.")]
        public string Password { get; set; }

        /// <summary>
        /// Represents the role or position of a user in the system.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        /// <summary>
        /// Collection navigation for related JobOpenings
        /// </summary>
        public ICollection<JobOpening> JobOpenings { get; set; } = new List<JobOpening>();
        
        /// <summary>
        /// Gets or sets the identity user.
        /// </summary>
        /// <value>
        /// The identity user.
        /// </value>
        public IdentityUser IdentityUser { get; set; }

        /// <summary>
        /// Gets or sets the user schedule.
        /// </summary>
        /// <value>
        /// The user schedule.
        /// </value>
        public UserSchedule? UserSchedule { get; set; }

        /// <summary>
        /// Gets or sets the interview.
        /// </summary>
        /// <value>
        /// The interview.
        /// </value>
        public Interview? Interview { get; set; }

        /// <summary>
        /// Gets or sets the examination.
        /// </summary>
        /// <value>
        /// The examination.
        /// </value>
        public Examination? Examination { get; set; }
    }
}

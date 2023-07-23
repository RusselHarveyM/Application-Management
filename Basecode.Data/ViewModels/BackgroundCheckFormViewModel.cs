﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class BackgroundCheckFormViewModel
    {
        public int CharReferenceId { get; set; }
        public int UserHRId { get; set; }

        [Required(ErrorMessage = "Firstname is required")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Lastname is required")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Relationship is required")]
        public string Relationship { get; set; }

        [Required(ErrorMessage = "Need to fill in")]
        public string? Q1 { get; set; }

        [Required(ErrorMessage = "Need to fill in")]
        public string? Q2 { get; set; }

        [Required(ErrorMessage = "Need to fill in")]
        public string? Q3 { get; set; }

        [Required(ErrorMessage = "Need to fill in")]
        public string? Q4 { get; set; }
    }
}

﻿using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.ViewModels
{
    public class ApplicantDetailsViewModel
    {
        public Applicant Applicant { get; set; }
        public List<CharacterReference> CharacterReferences { get; set; }
    }
}

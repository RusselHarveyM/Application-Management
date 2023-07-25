using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Models
{
    public class BackgroundCheck
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("CharacterReference")]
        public int CharReferenceId { get; set; }
        public int UserHRId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Relationship { get; set; }
        public DateTime AnsweredDate { get; set; }
        public string? Q1 { get; set; }
        public string? Q2 { get; set; }
        public string? Q3 { get; set; }
        public string? Q4 { get; set; }
        public bool IsSeen { get; set; }
        public CharacterReference CharacterReference { get; set; }
    }
}

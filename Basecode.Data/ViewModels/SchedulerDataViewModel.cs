using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels
{
    public class SchedulerDataViewModel
    {
        [Required(ErrorMessage = "Please select a job opening.")]
        [DisplayName("Job Opening")]
        public int JobOpeningId { get; set; }

        [Required(ErrorMessage = "Please select a type.")]
        [StringLength(50)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please set a date.")]
        public DateOnly Date { get; set; }

        public List<ApplicantSchedule>? ApplicantSchedules { get; set; }

        public class ApplicantSchedule
        {
            public int ApplicantId { get; set; }

            public string? Time { get; set; }
        }
    }
}

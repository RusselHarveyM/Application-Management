using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Basecode.Data.ViewModels
{
    public class SchedulerDataViewModel
    {
        [Required(ErrorMessage = "Please select a Job Opening.")]
        [DisplayName("Full Name")]
        public int JobOpeningId { get; set; }

        [Required(ErrorMessage = "Please select a Type.")]
        [StringLength(50)]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please select a Date.")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Please select an applicant and set a time.")]
        public List<ApplicantSchedule> ApplicantSchedules { get; set; }

        public class ApplicantSchedule
        {
            public int ApplicantId { get; set; }
            public string Time { get; set; }
        }
    }
}

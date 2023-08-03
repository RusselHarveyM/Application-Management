namespace Basecode.Data.ViewModels
{
    public class ApplicantExamViewModel
    {
        public int ApplicantId { get; set; }
        public string Firstname { get; set; }
        public string? Middlename { get; set; }
        public string Lastname { get; set; }
        public int ExamId { get; set; }
        public DateTime? ExaminationDate { get; set; }
    }
}

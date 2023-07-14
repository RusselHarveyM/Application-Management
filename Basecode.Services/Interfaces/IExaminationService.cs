using Basecode.Data.Models;

namespace Basecode.Services.Interfaces
{
    public interface IExaminationService
    {
        List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId);
        List<Examination> ShortlistExaminations(int jobOpeningId);
        double GetShortlistPercentage(int totalExams);
    }
}

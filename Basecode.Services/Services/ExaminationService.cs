using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class ExaminationService : ErrorHandling, IExaminationService
    {
        private readonly IExaminationRepository _repository;

        public ExaminationService(IExaminationRepository repository)
        {
            _repository = repository;
        }

        public List<Examination> GetAllExaminations()
        {
            return _repository.GetAllExaminations().ToList();
        }

    }
}

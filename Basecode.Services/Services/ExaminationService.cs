using Basecode.Data.Interfaces;
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
    }
}

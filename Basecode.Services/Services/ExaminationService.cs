using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services
{
    public class ExaminationService : ErrorHandling, IExaminationService
    {
        private readonly IExaminationRepository _repository;
        private readonly IApplicationService _applicationService;

        public ExaminationService(IExaminationRepository repository , IApplicationService applicationService)
        {
            _repository = repository;
            _applicationService = applicationService;
        }

        public List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId)
        {
            return _repository.GetExaminationsByJobOpeningId(jobOpeningId).ToList();
        }

    }
}

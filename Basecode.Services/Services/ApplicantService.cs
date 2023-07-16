using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository _repository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ITrackService _trackService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicantService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ApplicantService(IApplicantRepository repository, IApplicationRepository applicationRepository, IMapper mapper, ITrackService trackService)
        {
            _repository = repository;
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _trackService = trackService;
        }

        /// <summary>
        /// Gets the applicants.
        /// </summary>
        /// <returns></returns>
        public List<Applicant> GetApplicants()
        {
            return _repository.GetAll().ToList();
        }

        /// <summary>
        /// Retrieves an applicant by its ID.
        /// </summary>
        /// <param name="id">The ID of the applicant.</param>
        /// <returns>The Applicant object.</returns>
        public Applicant GetApplicantById(int id)
        {
            return _repository.GetById(id);
        }

        /// <summary>
        /// Creates a new applicant based on the provided applicant data.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns>Returns a tuple with the log content and the ID of the created applicant.</returns>
        public (LogContent, int) Create(ApplicantViewModel applicant)
        {
            LogContent logContent = new LogContent();
            int createdApplicantId = 0;

            logContent = CheckApplicant(applicant);
            if (logContent.Result == false)
            {
                var applicantModel = _mapper.Map<Applicant>(applicant);

                createdApplicantId = _repository.CreateApplicant(applicantModel);

                var application = new Application
                {
                    JobOpeningId = applicant.JobOpeningId,
                    ApplicantId = createdApplicantId,
                    Status = "For Screening",
                    ApplicationDate = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                _applicationRepository.CreateApplication(application);

                var newStatus = "Success";
                _trackService.UpdateTrackStatus(
                            applicantModel,
                            -1,
                            newStatus,
                            "Notify01"
                            );
            }

            return (logContent, createdApplicantId);
        }
    }
}

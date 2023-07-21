using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Basecode.Services.Services.ErrorHandling;

namespace Basecode.Services.Services
{
    public class ApplicantService : IApplicantService
    {
        private readonly IApplicantRepository _repository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly ITrackService _trackService;
        private readonly ResumeChecker _resumeChecker;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicantService"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ApplicantService(IApplicantRepository repository, IApplicationRepository applicationRepository, IMapper mapper, ITrackService trackService, ResumeChecker resumeChecker, IJobOpeningService jobOpeningService)
        {
            _repository = repository;
            _applicationRepository = applicationRepository;
            _mapper = mapper;
            _trackService = trackService;
            _resumeChecker = resumeChecker;
            _jobOpeningService = jobOpeningService;
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


        /// <summary>Updates the application.</summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        public void UpdateApplication(Application application, User user, string choice, string newStatus)
        {

            var applicant = _repository.GetById(application.ApplicantId);

            application.UpdateTime = DateTime.Now;
            application.Status = newStatus;

            if (choice.Equals("approved"))
            {
                _applicationRepository.UpdateApplication(application);
                _trackService.StatusNotification(applicant, user, newStatus);
            }
            else
            {
                //send automated email of regrets
                _trackService.UpdateTrackStatusEmail(applicant, application.Id, user.Id, "Rejected", "Rejected");
            }
        }

        /// <summary>
        /// Creates a new applicant based on the provided applicant data.
        /// </summary>
        /// <param name="applicant"></param>
        /// <returns>Returns a tuple with the log content and the ID of the created applicant.</returns>
        public async Task<(LogContent, int)> Create(ApplicantViewModel applicant)
        {
            LogContent logContent = new LogContent();
            int createdApplicantId = 0;
            var jobOpening = _jobOpeningService.GetById(applicant.JobOpeningId);

            logContent = CheckApplicant(applicant);
            if (logContent.Result == false)
            {
                var applicantModel = _mapper.Map<Applicant>(applicant);

                createdApplicantId = _repository.CreateApplicant(applicantModel);

                var application = new Application
                {
                    JobOpeningId = applicant.JobOpeningId,
                    ApplicantId = createdApplicantId,
                    Status = "NA",
                    ApplicationDate = DateTime.Now,
                    UpdateTime = DateTime.Now
                };

                var jobPosition = jobOpening.Title;

                var result = await _resumeChecker.CheckResume(jobPosition, applicant.CV);

                JsonDocument jsonDocument = JsonDocument.Parse(result);
                var jsonObject = jsonDocument.RootElement;

                // Accessing individual properties
                string jobPos = jsonObject.GetProperty("JobPosition").GetString();
                string score = jsonObject.GetProperty("Score").GetString();
                string explanation = jsonObject.GetProperty("Explanation").GetString();

                if (int.Parse(score.Replace("%", "")) > 60)
                {
                    application.Status = "Shortlisted";
                    var createdApplicationId = _applicationRepository.CreateApplication(application);

                    await _trackService.UpdateTrackStatusEmail(
                                applicantModel,
                                createdApplicationId,
                                -1,
                                "Shortlisted",
                                "GUID"
                                );
                }
                else
                {
                    await _trackService.RegretNotification(applicantModel, jobPosition);
                }

            }

            return (logContent, createdApplicantId);
        }

        /// <summary>
        /// Gets the applicants by the job opening id.
        /// </summary>
        /// <param name="jobOpeningId">The job opening id.</param>
        /// <returns></returns>
        public List<ApplicantStatusViewModel> GetApplicantsByJobOpeningId(int jobOpeningId)
        {
            return _repository.GetApplicantsByJobOpeningId(jobOpeningId)
                .Select(applicant => new ApplicantStatusViewModel
                {
                    Id = applicant.Id,
                    Firstname = applicant.Firstname,
                    Lastname = applicant.Lastname,
                    Status = applicant.Application.Status // Retrieve the Status from the related Application
                })
                .ToList();
        }

        /// <summary>
        /// Gets the applicants and statuses.
        /// </summary>
        /// <returns></returns>
        public List<ApplicantStatusViewModel> GetApplicantsWithStatuses()
        {
            return _repository.GetAll()
                .Select(applicant => new ApplicantStatusViewModel
                {
                    Id = applicant.Id,
                    Firstname = applicant.Firstname,
                    Lastname = applicant.Lastname,
                    Status = applicant.Application.Status, // Retrieve the Status from the related Application
                    JobOpeningId = applicant.Application.JobOpeningId,
                })
                .ToList();
        }
    }
}

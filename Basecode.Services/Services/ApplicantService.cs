using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Microsoft.EntityFrameworkCore;
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
        private readonly IApplicationService _applicationService;
        private readonly IJobOpeningService _jobOpeningService;
        private readonly ITrackService _trackService;
        private readonly IMapper _mapper;

        public ApplicantService(IApplicantRepository repository, IMapper mapper, ITrackService trackService, IJobOpeningService jobOpeningService, IApplicationService applicationService)
        {
            _repository = repository;
            _mapper = mapper;
            _trackService = trackService;
            _jobOpeningService = jobOpeningService;
            _applicationService = applicationService;
        }

        /// <summary>
        /// Gets the applicants.
        /// </summary>
        /// <returns>
        /// A list of Applicant objects.
        /// </returns>
        public List<Applicant> GetApplicants()
        {
            return _repository.GetAll().ToList();
        }

        /// <summary>
        /// Retrieves an applicant by its ID.
        /// </summary>
        /// <param name="id">The ID of the applicant.</param>
        /// <returns>
        /// The Applicant object.
        /// </returns>
        public Applicant GetApplicantById(int id)
        {

            return _repository.GetById(id);
        }

        /// <summary>
        /// Gets the applicant by identifier all.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Applicant GetApplicantByIdAll(int id)
        {
            return _repository.GetByIdAll(id);

        }


        /// <summary>
        /// Updates the application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="user">The user.</param>
        /// <param name="choice">The choice.</param>
        /// <param name="newStatus">The new status.</param>
        public async Task UpdateApplication(Application application, User user, string choice, string newStatus)
        {
           var result = await _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, newStatus);
            if(result != null)
            {
                _applicationService.Update(result);
            }
        }

        /// <summary>
        /// Creates a new applicant based on the provided applicant data.
        /// </summary>
        /// <param name="applicant">The applicant.</param>
        /// <returns>
        /// Returns a tuple with the log content and the ID of the created applicant.
        /// </returns>
        public async Task<(LogContent, int)> Create(ApplicantViewModel applicant)
        {
            LogContent logContent = new LogContent();
            int createdApplicantId = 0;
            var jobOpening = _jobOpeningService.GetByIdClean(applicant.JobOpeningId);

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
                    UpdateTime = DateTime.Now,
                    Applicant = applicantModel,
                    JobOpening = jobOpening
                };

                _applicationService.Create(application);

                // Resume checking moved to the TrackService
                var result = await _trackService.CheckAndSendApplicationStatus(application, applicantModel, jobOpening);

                if(result != null)
                {
                    _applicationService.Update(result);
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
        /// Gets the applicants with rejected or no schedule.
        /// </summary>
        /// <returns></returns>
        public List<ApplicantStatusViewModel> GetApplicantsWithRejectedOrNoSchedule()
        {
            var rejectedApplicantIds = _repository.GetAll()
                .Where(applicant => applicant.Application.UserSchedule.Status == "rejected")
                .Select(applicant => applicant.Id)
                .ToList();

            if(rejectedApplicantIds.Count > 0)
            return _repository.GetAll()
                .Where(applicant =>
                    // Applicants with no UserSchedule record at all
                    applicant.Application.UserSchedule == null ||
                    // Applicants with UserSchedule records having "rejected" status
                    rejectedApplicantIds.Contains(applicant.Id))
                .Select(applicant => new ApplicantStatusViewModel
                {
                    Id = applicant.Id,
                    Firstname = applicant.Firstname,
                    Lastname = applicant.Lastname,
                    Status = applicant.Application.Status,
                    JobOpeningId = applicant.Application.JobOpeningId,
                })
                .ToList();
            return new List<ApplicantStatusViewModel>();
        }

        /// <summary>
        /// Gets the applicant by application identifier.
        /// </summary>
        /// <param name="applicationId">The application identifier.</param>
        /// <returns></returns>
        public Applicant GetApplicantByApplicationId(Guid applicationId)
        {
            return _repository.GetApplicantByApplicationId(applicationId);
        }

        /// <summary>
        /// This method retrieves a list of applicant names along with their corresponding email addresses and job titles.
        /// </summary>
        /// <returns>A list of tuples, each containing the applicant's full name, email address, and job title.</returns>
        public List<(string Name, string Email, string Title)> GetApplicantNameAndJobTitle()
        {
            return _repository.GetApplicantNameAndJobTitle();
        }
    }
}

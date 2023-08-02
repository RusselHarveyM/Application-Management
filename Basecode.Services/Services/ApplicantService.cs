﻿using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Hangfire;
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
        public void UpdateApplication(Application application, User user, string choice, string newStatus)
        {
           var result = _trackService.UpdateApplicationStatusByEmailResponse(application, user, choice, newStatus);
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
        public (LogContent, int) Create(ApplicantViewModel applicant)
        {
            LogContent logContent = new LogContent();
            int createdApplicantId = 0;
            var jobOpeningClone = _jobOpeningService.GetByIdClean(applicant.JobOpeningId);

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
                    JobOpening = jobOpeningClone
                };

                _applicationService.Create(application);
                
                BackgroundJob.Enqueue(() => CheckAndSendApplicationStatus(application.Id)); 
            }

            return (logContent, createdApplicantId);
        }

        public async Task CheckAndSendApplicationStatus(Guid applicationId)
        {
            var application = _applicationService.GetApplicationWithAllRelationsById(applicationId);
            if (application != null)
            {
                // Resume checking moved to the TrackService
                var result = await _trackService.CheckAndSendApplicationStatus(application);
                if (result != null)
                {
                    _applicationService.Update(result);
                }
            } 
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
        
        public List<Applicant> GetApplicantsByJobOpeningIdApplicant(int jobOpeningId)
        {
            return _repository.GetApplicantsByJobOpeningId(jobOpeningId).ToList();
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

            return _repository.GetAll()
                .Where(applicant =>
                    // Applicants with no UserSchedule record at all
                    (applicant.Application.UserSchedule == null ||
                    // Applicants with UserSchedule records having "rejected" status
                    rejectedApplicantIds.Contains(applicant.Id)))
                .Select(applicant => new ApplicantStatusViewModel
                {
                    Id = applicant.Id,
                    Firstname = applicant.Firstname,
                    Lastname = applicant.Lastname,
                    Status = applicant.Application.Status,
                    JobOpeningId = applicant.Application.JobOpeningId,
                })
                .ToList();
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
        /// Retrieves a list of applicants along with their associated job openings and character references from the database.
        /// </summary>
        /// <returns>A List of Applicant objects, each containing their respective Application (including JobOpening) and CharacterReferences.</returns>
        public List<Applicant> GetApplicantsWithJobAndReferences(string userAspId)
        {
            return _repository.GetApplicantsWithJobAndReferences(userAspId);
        }
    }
}

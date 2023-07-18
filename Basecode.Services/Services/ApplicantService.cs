﻿using AutoMapper;
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


        /// <summary>Updates the application.</summary>
        /// <param name="applicant">The applicant.</param>
        /// <param name="newStatus">The new status.</param>
        public void UpdateApplication(Application application,User user, string choice, string newStatus)
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

                var createdApplicationId = _applicationRepository.CreateApplication(application);

                _trackService.UpdateTrackStatusEmail(
                            applicantModel,
                            createdApplicationId,
                            - 1,
                            "For Screening",
                            "GUID"
                            );
            }

            return (logContent, createdApplicantId);
        }
    }
}

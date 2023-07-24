﻿using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Services.Services
{

    public class ApplicationService : ErrorHandling, IApplicationService
    {

        private readonly IApplicationRepository _repository;
        private readonly IMapper _mapper;

        public ApplicationService(IApplicationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        public void Create(Application application)
        {
            _repository.CreateApplication(application);
        }

        /// <summary>
        /// Creates the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public Guid CreateWithId(Application application)
        {
            return _repository.CreateApplication(application);
        }

        /// <summary>
        /// Retrieves an application by its ID.
        /// </summary>
        /// <param name="id">The ID of the application to retrieve.</param>
        /// <returns>
        /// The application with the specified ID, or null if not found.
        /// </returns>
        public ApplicationViewModel? GetById(Guid id)
        {
            var application = _repository.GetById(id);

            if (application == null)
            {
                return null;
            }

            //var job = _jobOpeningService.GetById(application.JobOpeningId);
            //var applicant = _applicantService.GetApplicantById(application.ApplicantId);

            var applicationViewModel = _mapper.Map<ApplicationViewModel>(application);
            applicationViewModel.JobOpeningTitle = application.JobOpening.Title;
            applicationViewModel.ApplicantName = $"{application.Applicant.Firstname} {application.Applicant.Lastname}";

            return applicationViewModel;
        }


        /// <summary>
        /// Gets the application by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Application GetApplicationById(Guid id)
        {
            var application = _repository.GetById(id);

            if (application == null)
            {
                return null;
            }

            return application;
        }

        /// <summary>
        /// Gets the shorlisted applicatons.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns></returns>
        public List<Application> GetShorlistedApplicatons(string stage)
        {
            var data = _repository.GetAll()
                .Include(a => a.JobOpening)
                .Include(a => a.Applicant)
                .Where(m => m.Status == stage)
                .ToList();
            return data;
        }



        /// <summary>
        /// Updates the specified application.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        public LogContent Update(Application application)
        {
            var existingApplication = _repository.GetById(application.Id);

            LogContent logContent = new LogContent();
            logContent = CheckApplication(existingApplication);

            if (logContent.Result == false)
            {
                existingApplication.Status = application.Status;
                existingApplication.UpdateTime = DateTime.Now;

                _repository.UpdateApplication(existingApplication);
            }

            return logContent;
        }

        /// <summary>
        /// Gets the applications by ids.
        /// </summary>
        /// <param name="applicationIds">The application ids.</param>
        /// <returns></returns>
        public List<Application> GetApplicationsByIds(List<Guid> applicationIds)
        {
            return _repository.GetApplicationsByIds(applicationIds);
        }

        /// <summary>
        /// Gets the application id based on the applicant id.
        /// </summary>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <returns></returns>
        public Guid GetApplicationIdByApplicantId(int applicantId)
        {
            return _repository.GetApplicationIdByApplicantId(applicantId);
        }

        /// <summary>
        /// Gets the application by applicant identifier.
        /// </summary>
        /// <param name="applicantId">The applicant identifier.</param>
        /// <returns></returns>
        public Application? GetApplicationByApplicantId(int applicantId)
        {
            return _repository.GetAll().Where(m => m.ApplicantId == applicantId).SingleOrDefault();
        }
    }
}
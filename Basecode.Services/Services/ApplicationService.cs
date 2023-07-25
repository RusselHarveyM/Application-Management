using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
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
        private readonly IJobOpeningService _jobOpeningService;
        private readonly IApplicantService _applicantService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="jobOpeningService">The job opening service.</param>
        /// <param name="applicantService">The applicant service.</param>
        /// <param name="emailService">The Email Service</param>
        public ApplicationService(IApplicationRepository repository, IMapper mapper, IJobOpeningService jobOpeningService, IApplicantService applicantService, IEmailService emailService)
        {
            _repository = repository;
            _jobOpeningService = jobOpeningService;
            _applicantService = applicantService;
            _mapper = mapper;
            _emailService = emailService;
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

            var job = _jobOpeningService.GetById(application.JobOpeningId);
            var applicant = _applicantService.GetApplicantById(application.ApplicantId);

            var applicationViewModel = _mapper.Map<ApplicationViewModel>(application);
            applicationViewModel.JobOpeningTitle = job.Title;
            applicationViewModel.ApplicantName = $"{applicant.Firstname} {applicant.Lastname}";

            return applicationViewModel;
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
        /// Updates the application status of an applicant in the database and notifies the HR and the applicant via email.
        /// </summary>
        /// <param name="applicantId">The ID of the applicant.</param>
        /// <param name="newStatus">The new status to update.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateApplicationStatus(int applicantId, string newStatus)
        {
            // Update applicant status in the database
            // ongoing!!!
            newStatus = "Success"; //partial

            // Get applicant details from the database
            Applicant applicant = _applicantService.GetApplicantById(applicantId);

            // Notify HR
            await _emailService.SendEmail("hrautomatesystem@outlook.com", "Applicant Status Update for HR",
                $"Applicant {applicant.Firstname} (ID: {applicant.Id}) has changeds status to {newStatus}.");

            // Notify the applicant
            await _emailService.SendEmail(applicant.Email, "Application Status Update for Applicant",
                $"Dear {applicant.Firstname},\n\nYour application status has been updated to {newStatus}.\n\nThank you.");
        }

    }
}
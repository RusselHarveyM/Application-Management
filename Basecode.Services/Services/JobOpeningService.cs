using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basecode.Services.Services
{
    public class JobOpeningService : ErrorHandling, IJobOpeningService
    {
        private readonly IJobOpeningRepository _repository;
        private readonly IQualificationService _qualificationService;
        private readonly IResponsibilityService _responsibilityService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobOpeningService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="qualificationService">The qualification service.</param>
        /// <param name="responsibilityService">The responsibility service.</param>
        public JobOpeningService(IJobOpeningRepository repository, IMapper mapper, IQualificationService qualificationService, IResponsibilityService responsibilityService)
        {
            _repository = repository;
            _mapper = mapper;
            _qualificationService = qualificationService;
            _responsibilityService = responsibilityService;
        }

        /// <summary>
        /// Gets a list of all job openings.
        /// </summary>
        /// <returns>
        /// A list of job opening view models.
        /// </returns>
        public List<JobOpeningViewModel> GetJobs()
        {
            var data = _repository.GetAll()
                .Select(m => _mapper.Map<JobOpeningViewModel>(m))
                .ToList();

            return data;
        }

        /// <summary>
        /// Creates a new job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening to create.</param>
        /// <param name="createdBy">The user who created the job opening.</param>
        /// <returns></returns>
        public LogContent Create(JobOpeningViewModel jobOpening, string createdBy)
        {
            LogContent logContent = new LogContent();

            logContent = CheckJobOpening(jobOpening);
            if (logContent.Result == false)
            {
                var jobOpeningModel = _mapper.Map<JobOpening>(jobOpening);
                jobOpeningModel.CreatedBy = createdBy;
                jobOpeningModel.CreatedTime = DateTime.Now;
                jobOpeningModel.UpdatedBy = createdBy;
                jobOpeningModel.UpdatedTime = DateTime.Now;

                _repository.AddJobOpening(jobOpeningModel);
            }

            return logContent;
        }

        /// <summary>
        /// Gets a job opening by its ID.
        /// </summary>
        /// <param name="id">The ID of the job opening to get.</param>
        /// <returns>
        /// A job opening view model, or null if no such job opening exists.
        /// </returns>
        public JobOpeningViewModel GetById(int id)
        {
            var qualifications = _qualificationService.GetQualificationsByJobOpeningId(id);
            var responsibilities = _responsibilityService.GetResponsibilitiesByJobOpeningId(id);

            var data = _repository.GetAll()
                .Where(m => m.Id == id)
                .Select(m => new JobOpeningViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    EmploymentType = m.EmploymentType,
                    WorkSetup = m.WorkSetup,
                    Location = m.Location,
                    Responsibilities = responsibilities,
                    Qualifications = qualifications
                })
                .FirstOrDefault();

            return data;
        }

        /// <summary>
        /// Updates an existing job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening to update.</param>
        /// <param name="updatedBy">The user who updated the job opening.</param>
        /// <returns></returns>
        public LogContent Update(JobOpeningViewModel jobOpening, string updatedBy)
        {
            LogContent logContent = new LogContent();
            logContent = CheckJobOpening(jobOpening);
            if (logContent.Result == false)
            {
                _responsibilityService.DeleteResponsibilitiesByJobOpeningId(jobOpening.Id);
                _qualificationService.DeleteQualificationsByJobOpeningId(jobOpening.Id);

                var jobExisting = _repository.GetJobOpeningById(jobOpening.Id);

                _mapper.Map(jobOpening, jobExisting);
                jobExisting.UpdatedBy = updatedBy;
                jobExisting.UpdatedTime = DateTime.Now;

                // Update qualifications and responsibilities
                jobExisting.Responsibilities = jobOpening.Responsibilities ?? new List<Responsibility>();
                jobExisting.Qualifications = jobOpening.Qualifications ?? new List<Qualification>();

                _repository.UpdateJobOpening(jobExisting);
            }
            return logContent;
        }




        /// <summary>
        /// Deletes a job opening.
        /// </summary>
        /// <param name="jobOpening">The job opening to delete.</param>
        public void Delete(JobOpeningViewModel jobOpening)
        {
            var job = _mapper.Map<JobOpening>(jobOpening);
            _repository.DeleteJobOpening(job);
        }

        /// <summary>
        /// Gets all job opening ids.
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllJobOpeningIds()
        {
            return _repository.GetAllJobOpeningIds();
        }
    }
}

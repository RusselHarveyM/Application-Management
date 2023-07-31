using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.Repositories;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Basecode.Services.Util;
using Microsoft.AspNetCore.Identity;
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
        private readonly IUserRepository _userRepository;
        private readonly IEmailSchedulerService _emailSchedulerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobOpeningService" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="qualificationService">The qualification service.</param>
        /// <param name="responsibilityService">The responsibility service.</param>
        public JobOpeningService(IJobOpeningRepository repository, IMapper mapper, IQualificationService qualificationService, IResponsibilityService responsibilityService, 
            IUserRepository userRepository, IEmailSchedulerService emailSchedulerService)
        {
            _repository = repository;
            _mapper = mapper;
            _qualificationService = qualificationService;
            _responsibilityService = responsibilityService;
            _userRepository = userRepository;
            _emailSchedulerService = emailSchedulerService;
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
        /// <returns>The log content and the new job opening's id.</returns>
        public (LogContent, int) Create(JobOpeningViewModel jobOpening, string createdBy)
        {
            LogContent logContent = new LogContent();
            int jobOpeningId = 0;

            logContent = CheckJobOpening(jobOpening);
            if (logContent.Result == false)
            {
                var jobOpeningModel = _mapper.Map<JobOpening>(jobOpening);
                jobOpeningModel.CreatedBy = createdBy;
                jobOpeningModel.CreatedTime = DateTime.Now;
                jobOpeningModel.UpdatedBy = createdBy;
                jobOpeningModel.UpdatedTime = DateTime.Now;

                jobOpeningId = _repository.AddJobOpening(jobOpeningModel);
            }

            return (logContent, jobOpeningId);
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
                    Tag = m.Tag,
                    Responsibilities = responsibilities,
                    Qualifications = qualifications
                })
                .FirstOrDefault();

            return data;
        }

        public JobOpening GetByIdClean(int id)
        {
            return _repository.GetJobOpeningById(id);
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

        /// <summary>
        /// Gets the jobs with related applications.
        /// </summary>
        /// <returns>A list of JobOpeningViewModels.</returns>
        public List<JobOpeningViewModel> GetJobsWithApplications()
        {
            var data = _repository.GetJobsWithApplications()
                .Select(m => _mapper.Map<JobOpeningViewModel>(m))
                .ToList();
            return data;
        }

        /// <summary>
        /// Gets the job opening title by its id.
        /// </summary>
        /// <param name="id">The job opening id.</param>
        /// <returns>The job opening title.</returns>
        public string GetJobOpeningTitleById(int id)
        {
            return _repository.GetJobOpeningTitleById(id);
        }

        /// <summary>
        /// Gets the related user ids.
        /// </summary>
        /// <param name="jobOpeningId">The job opening id.</param>
        /// <returns>A list of user ids.</returns>
        public List<string> GetLinkedUserIds(int jobOpeningId)
        {
            return _repository.GetLinkedUserIds(jobOpeningId).ToList();
        }

        /// <summary>
        /// Retrieves the list of user IDs associated with a specific job opening user.
        /// </summary>
        /// <param name="jobOpeningId">The ID of the job opening..</param>
        /// <returns>A list of integers. </returns>
        public List<int> GetUserById(int jobOpeningId)
        {
            return _repository.GetUserById(jobOpeningId).ToList();
        }

        /// <summary>
        /// Updates the many-to-many relationship between User and JobOpening.
        /// </summary>
        /// <param name="jobOpeningId">The job opening id.</param>
        /// <param name="assignedUserIds">The assigned user ids.</param>
        public void UpdateJobOpeningUsers(int jobOpeningId, List<string> assignedUserIds)
        {
            // Retrieve the current assigned user IDs from the database
            var currentAssignedUserIds = _repository.GetLinkedUserIds(jobOpeningId);

            // Check if there are any new assigned users
            bool hasNewAssignedUsers = assignedUserIds.Except(currentAssignedUserIds).Any();

            _repository.UpdateJobOpeningUsers(jobOpeningId, assignedUserIds);

            // Only call GetReminder if new users are added (not removed)
            if (hasNewAssignedUsers)
            {
                GetReminder(jobOpeningId);
            }
        }

        /// <summary>
        /// Retrieves a reminder for a specific job and sends an email notification.
        /// </summary>
        /// <param name="id">The unique identifier of the job</param>
        public void GetReminder(int id)
        {

            var job = GetById(id);
            var data = _repository.GetUserById(id);
            int user = data.OrderBy(x => x).Last();
            var per = _userRepository.GetById(user);
            switch (per.Role)
            {
                case "Deployment Team":
                    _emailSchedulerService.ScheduleForDT(per.Email, per.Fullname, per.Username, per.Password, job.Title);
                    break;

                case "Human Resources":
                    _emailSchedulerService.ScheduleForHR(per.Email, per.Fullname, per.Username, per.Password, job.Title);
                    break;

                case "Technical":
                    _emailSchedulerService.ScheduleForTechnical(per.Email, per.Fullname, per.Username, per.Password, job.Title);
                    break;

                default:
                    // Handle the default case if the role doesn't match any case
                    break;
            }
            //Use switch case for different roles
        }
    }
}

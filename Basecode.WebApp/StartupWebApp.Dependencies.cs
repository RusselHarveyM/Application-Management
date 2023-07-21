﻿using Basecode.WebApp.Authentication;
using Basecode.Data;
using Basecode.Data.Interfaces;
using Basecode.Data.Repositories;
using Basecode.Services.Interfaces;
using Basecode.Services.Services;
using Basecode.Data.Models;
using Basecode.Services.Util;

namespace Basecode.WebApp
{
    public partial class StartupWebApp
    {
        private void ConfigureDependencies(IServiceCollection services)
        {
            // Common
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ClaimsProvider, ClaimsProvider>();
            services.AddScoped<ResumeChecker>();


            // Services 
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IApplicantService, ApplicantService>();
            services.AddScoped<IJobOpeningService, JobOpeningService>();
            services.AddScoped<IApplicationService, ApplicationService>();
            services.AddScoped<IQualificationService, QualificationService>();
            services.AddScoped<IResponsibilityService, ResponsibilityService>();
            services.AddScoped<ICharacterReferenceService, CharacterReferenceService>();
            services.AddScoped<IExaminationService, ExaminationService>();
            services.AddScoped<IShortlistingService, ShortlistingService>();
            services.AddScoped<ITrackService, TrackService>();
            services.AddScoped<IAssignInterviewerService, AssignInterviewerService>();
            services.AddScoped<IUserScheduleService, UserScheduleService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IApplicantRepository, ApplicantRepository>();
            services.AddScoped<IJobOpeningRepository, JobOpeningRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IQualificationRepository, QualificationRepository>();
            services.AddScoped<IResponsibilityRepository, ResponsibilityRepository>();
            services.AddScoped<ICharacterReferenceRepository, CharacterReferenceRepository>();
            services.AddScoped<IExaminationRepository, ExaminationRepository>();
            services.AddScoped<IAssignInterviewerRepository, AssignInterviewerRepository>();
            services.AddScoped<IUserScheduleRepository, UserScheduleRepository>();
        }
    }
}
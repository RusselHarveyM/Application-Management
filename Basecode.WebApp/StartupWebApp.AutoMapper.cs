using AutoMapper;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Basecode.WebApp
{
    public partial class StartupWebApp
    {
        private void ConfigureMapper(IServiceCollection services)
        {
            var Config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<JobOpening, JobOpeningViewModel>();
                cfg.CreateMap<JobOpeningViewModel, JobOpening>();
                cfg.CreateMap<Application, ApplicationViewModel>();
                cfg.CreateMap<User, LoginViewModel>();
                cfg.CreateMap<ApplicantViewModel, Applicant>();
                cfg.CreateMap<Applicant, ApplicantViewModel>();
                cfg.CreateMap<CharacterReferenceViewModel, CharacterReference>();
                cfg.CreateMap<CharacterReference, CharacterReferenceViewModel>();
                cfg.CreateMap<BackgroundCheckFormViewModel, BackgroundCheck>();
                cfg.CreateMap<UserViewModel, User>();
                cfg.CreateMap<UserUpdateViewModel, User>();
            });

            services.AddSingleton(Config.CreateMapper());
        }
    }
}
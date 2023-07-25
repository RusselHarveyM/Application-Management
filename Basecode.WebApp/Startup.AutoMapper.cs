using AutoMapper;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Basecode.WebApp
{
    public partial class Startup
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
            });

            services.AddSingleton(Config.CreateMapper());
        }
    }
}
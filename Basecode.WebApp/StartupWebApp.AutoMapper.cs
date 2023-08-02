using AutoMapper;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;

namespace Basecode.WebApp;

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

            cfg.CreateMap<Applicant, Applicant>()
                .ForMember(dest => dest.Application, opt => opt.Ignore());

            cfg.CreateMap<UserSchedule, UserSchedule>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Application, opt => opt.Ignore());

            cfg.CreateMap<BackgroundCheck, BackgroundCheck>()
                .ForMember(dest => dest.CharacterReference, opt => opt.Ignore());

            cfg.CreateMap<User, User>()
                .ForMember(dest => dest.JobOpenings, opt => opt.Ignore())
                .ForMember(dest => dest.IdentityUser, opt => opt.Ignore())
                .ForMember(dest => dest.UserSchedule, opt => opt.Ignore())
                .ForMember(dest => dest.Interview, opt => opt.Ignore())
                .ForMember(dest => dest.Examination, opt => opt.Ignore())
                .ForMember(dest => dest.BackgroundCheck, opt => opt.Ignore());
        });

        services.AddSingleton(Config.CreateMapper());
    }
}
using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Basecode.Services.Services;

public class ApplicationService : ErrorHandling, IApplicationService
{
    private readonly IMapper _mapper;

    private readonly IApplicationRepository _repository;

    public ApplicationService(IApplicationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    ///     Creates the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    public void Create(Application application)
    {
        _repository.CreateApplication(application);
    }

    /// <summary>
    ///     Creates the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <returns></returns>
    public Guid CreateWithId(Application application)
    {
        return _repository.CreateApplication(application);
    }

    /// <summary>
    ///     Retrieves an application by its ID.
    /// </summary>
    /// <param name="id">The ID of the application to retrieve.</param>
    /// <returns>
    ///     The application with the specified ID, or null if not found.
    /// </returns>
    public ApplicationViewModel? GetById(Guid id)
    {
        var application = _repository.GetById(id);

        if (application == null) return null;

        //var job = _jobOpeningService.GetById(application.JobOpeningId);
        //var applicant = _applicantService.GetApplicantById(application.ApplicantId);

        var applicationViewModel = _mapper.Map<ApplicationViewModel>(application);
        applicationViewModel.JobOpeningTitle = application.JobOpening.Title;
        applicationViewModel.ApplicantName = $"{application.Applicant.Firstname} {application.Applicant.Lastname}";

        return applicationViewModel;
    }


    /// <summary>
    ///     Gets the application by identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public Application GetApplicationById(Guid id)
    {
        var application = _repository.GetById(id);

        if (application == null) return null;

        return application;
    }

    /// <summary>
    /// Get Total Applications
    /// </summary>
    /// <returns></returns>
    public int GetTotalApplications()
    {
        return _repository.GetAll().Count();
    }

    /// <summary>
    /// Get Total Deployed Applications
    /// </summary>
    /// <returns></returns>
    public int GetDeployed()
    {
        return _repository.GetAll().Where(m => m.Status == "Deployed").Count();
    }

    /// <summary>
    /// Get Total Onboarded Applications
    /// </summary>
    /// <returns></returns>
    public int GetOnboarded()
    {
        return _repository.GetAll().Where(m => m.Status == "Onboarded").Count();
    }

    /// <summary>
    ///     Gets the application with complete relations by identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    public Application? GetApplicationWithAllRelationsById(Guid applicationId)
    {
        var application = _repository.GetApplicationWithAllRelationsById(applicationId);
        if (application == null) return null;
        return application;
    }

    /// <summary>
    ///     Gets the shorlisted applicatons.
    /// </summary>
    /// <param name="stage">The stage.</param>
    /// <returns></returns>
    public List<Application> GetShorlistedApplicatons(string stage, int jobId)
    {
        var data = _repository.GetAll()
            .Include(a => a.JobOpening)
            .Include(a => a.Applicant)
            .Where(m => m.Status == stage)
            .Where(m => m.JobOpeningId == jobId)
            .ToList();
        return data;
    }


    /// <summary>
    ///     Updates the specified application.
    /// </summary>
    /// <param name="application">The application.</param>
    /// <returns></returns>
    public LogContent Update(Application application)
    {
        var logContent = new LogContent();
        logContent = CheckApplication(application);

        if (logContent.Result == false) _repository.UpdateApplication(application);

        return logContent;
    }

    /// <summary>
    ///     Gets the applications by ids.
    /// </summary>
    /// <param name="applicationIds">The application ids.</param>
    /// <returns></returns>
    public List<Application> GetApplicationsByIds(List<Guid> applicationIds)
    {
        return _repository.GetApplicationsByIds(applicationIds);
    }

    /// <summary>
    ///     Gets the application id based on the applicant id.
    /// </summary>
    /// <param name="applicantId">The applicant identifier.</param>
    /// <returns></returns>
    public Guid GetApplicationIdByApplicantId(int applicantId)
    {
        return _repository.GetApplicationIdByApplicantId(applicantId);
    }

    /// <summary>
    ///     Gets the application by applicant identifier.
    /// </summary>
    /// <param name="applicantId">The applicant identifier.</param>
    /// <returns></returns>
    public Application? GetApplicationByApplicantId(int applicantId)
    {
        return _repository.GetAll().Where(m => m.ApplicantId == applicantId).SingleOrDefault();
    }
}
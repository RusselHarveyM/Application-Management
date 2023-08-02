using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;

namespace Basecode.Services.Services;

public class ExaminationService : ErrorHandling, IExaminationService
{
    private readonly IApplicationService _applicationService;
    private readonly IExaminationRepository _repository;

    public ExaminationService(IExaminationRepository repository, IApplicationService applicationService)
    {
        _repository = repository;
        _applicationService = applicationService;
    }

    /// <summary>
    ///     Gets the examinations by the job opening ID
    /// </summary>
    /// <param name="jobOpeningId">The job opening ID.</param>
    /// <returns></returns>
    public List<Examination> GetExaminationsByJobOpeningId(int jobOpeningId)
    {
        return _repository.GetExaminationsByJobOpeningId(jobOpeningId).ToList();
    }

    /// <summary>
    ///     Adds the examination.
    /// </summary>
    /// <param name="schedule">The schedule.</param>
    /// <returns></returns>
    public LogContent AddExamination(UserSchedule schedule, string teamsLink)
    {
        var logContent = CheckUserSchedule(schedule);

        if (logContent.Result == false)
        {
            var examination = new Examination
            {
                ApplicationId = schedule.ApplicationId,
                UserId = schedule.UserId,
                Date = schedule.Schedule,
                TeamsLink = teamsLink
            };

            _repository.AddExamination(examination);
        }

        return logContent;
    }

    /// <summary>
    /// Gets the examination by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    public Examination GetExaminationByApplicationId(Guid applicationId)
    {
        return _repository.GetExaminationByApplicationId(applicationId);
    }
}
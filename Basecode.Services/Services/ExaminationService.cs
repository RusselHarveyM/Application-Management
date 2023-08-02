using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Services.Interfaces;
using NLog;

namespace Basecode.Services.Services;

public class ExaminationService : ErrorHandling, IExaminationService
{
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IApplicationService _applicationService;
    private readonly IExaminationRepository _repository;
    private readonly ITrackService _trackService;
    private readonly IUserService _userService;

    public ExaminationService(IExaminationRepository repository, IApplicationService applicationService, ITrackService trackService, IUserService userService)
    {
        _repository = repository;
        _applicationService = applicationService;
        _trackService = trackService;
        _userService = userService;
    }

    /// <summary>
    ///     Gets the examinations by the job opening ID
    /// </summary>
    /// <param name="jobOpeningId">The job opening ID.</param>
    /// <returns></returns>
    public List<Examination> GetShortlistableExamsByJobOpeningId(int jobOpeningId)
    {
        return _repository.GetShortlistableExamsByJobOpeningId(jobOpeningId).ToList();
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

    /// <summary>
    /// Updates the examination score.
    /// </summary>
    /// <param name="examinationId">The examination identifier.</param>
    /// <param name="score">The score.</param>
    /// <returns></returns>
    public LogContent UpdateExaminationScore(int examinationId, int score)
    {
        var logContent = new LogContent();
        var examination = _repository.GetExaminationById(examinationId);

        if (examination != null)
        {
            var result = "Fail";
            if (score >= 70) result = "Pass";

            examination.Score = score;
            examination.Result = result;

            logContent = CheckExamination(examination);
            if (!logContent.Result)
            {
                _repository.UpdateExamination(examination);
                UpdateApplicationStatusByResult(examination, result);
            }
        }

        return logContent;
    }

    /// <summary>
    /// Updates the application status based on the exam result.
    /// </summary>
    /// <param name="examination">The examination.</param>
    /// <param name="result">The result.</param>
    public void UpdateApplicationStatusByResult(Examination examination, string result)
    {
        var application = _applicationService.GetApplicationById(examination.ApplicationId);
        var user = _userService.GetById(examination.UserId);

        if (result == "Pass")
            application = _trackService.UpdateApplicationStatus(application, user, "For Technical Interview", string.Empty);
        else
            application = _trackService.UpdateApplicationStatus(application, user, "Rejected", "Regret");
           
        if (application != null)
        {
            var logContent = _applicationService.Update(application);
            if (!logContent.Result)
                _logger.Trace($"Successfully updated application [ {application.Id} ].");
            else
                _logger.Error(SetLog(logContent));
        }
    }
}
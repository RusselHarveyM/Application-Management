using AutoMapper;
using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Basecode.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Basecode.Services.Services;

public class UserService : ErrorHandling, IUserService
{
    private readonly IJobOpeningService _jobOpeningService;
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UserService" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public UserService(IUserRepository repository, IJobOpeningService jobOpeningService, IMapper mapper)
    {
        _repository = repository;
        _jobOpeningService = jobOpeningService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Retrieves a list of all users.
    /// </summary>
    /// <returns>
    ///     A list of UserViewModel objects representing all users available.
    /// </returns>
    public List<UserViewModel> RetrieveAll()
    {
        var data = _repository.RetrieveAll().Select(s => new UserViewModel
        {
            Id = s.Id,
            Fullname = s.Fullname,
            Username = s.Username,
            Email = s.Email,
            Role = s.Role
        }).ToList();

        return data;
    }

    /// <summary>
    ///     Creates the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <response code="400">User details are invalid</response>
    public async Task<LogContent> Create(UserViewModel user)
    {
        var logContent = new LogContent();
        logContent = CheckUser(user);

        if (logContent.Result == false)
        {
            var mapUser = _mapper.Map<User>(user);
            await _repository.Create(mapUser);
        }

        return logContent;
    }

    /// <summary>
    ///     Retrieves a specific user based on the provided ID.
    /// </summary>
    /// <param name="id">Represents the ID of the user to fetch.</param>
    /// <returns>
    ///     A User object corresponding to the matching ID.
    /// </returns>
    public User GetById(int id)
    {
        return _repository.GetById(id);
    }

    /// <summary>
    ///     Gets the by identifier asynchronous.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns></returns>
    public async Task<User> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public User GetByEmail(string email)
    {
        return _repository.GetByEmail(email);
    }

    /// <summary>
    ///     Updates the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns></returns>
    public async Task<LogContent> Update(UserUpdateViewModel user)
    {
        var logContent = new LogContent();
        logContent = CheckUser(user);

        if (logContent.Result == false)
        {
            var userToBeUpdated = _mapper.Map<User>(user);
            await _repository.Update(userToBeUpdated);
        }

        return logContent;
    }

    /// <summary>
    ///     Deletes the specified user.
    /// </summary>
    /// <param name="user">The user.</param>
    public async Task Delete(User user)
    {
        await _repository.Delete(user);
    }

    /// <summary>
    ///     Gets the validation errors.
    /// </summary>
    /// <param name="modelState">State of the model.</param>
    /// <returns>Dictionary containing the validation errors.</returns>
    /// <exception cref="Basecode.Data.Constants.Exception">ModelState is empty</exception>
    public Dictionary<string, string> GetValidationErrors(ModelStateDictionary modelState)
    {
        var validationErrors = new Dictionary<string, string>();

        foreach (var key in modelState.Keys)
        {
            var modelStateEntry = modelState[key];

            foreach (var error in modelStateEntry.Errors) validationErrors.Add(key, error.ErrorMessage);
        }

        return validationErrors;
    }

    /// <summary>
    ///     Gets all users and their link status to a job opening.
    /// </summary>
    /// <param name="jobOpeningId">The job opening id.</param>
    /// <returns></returns>
    public List<HRUserViewModel> GetAllUsersWithLinkStatus(int jobOpeningId)
    {
        var allUsers = _repository.RetrieveAll();

        // Get the users linked to the specified JobOpening
        var linkedUserIds =
            _jobOpeningService
                .GetLinkedUserIds(
                    jobOpeningId); // Replace _jobOpeningRepository with your actual repository instance to get linked users

        // Create a new HRUserViewModel list with link status
        var usersWithLinkStatus = allUsers.Select(user => new HRUserViewModel
        {
            Id = user.Id,
            Fullname = user.Fullname,
            Email = user.Email,
            IsLinkedToJobOpening = linkedUserIds.Contains(user.AspId),
            AspId = user.AspId
        }).ToList();

        return usersWithLinkStatus;
    }

    /// <summary>
    ///     Gets the linked job openings.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    public List<JobOpeningBasicViewModel> GetLinkedJobOpenings(string userAspId)
    {
        return _repository.GetLinkedJobOpenings(userAspId).ToList();
    }

    /// <summary>
    ///     Finds the user asynchronous.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public Task<IdentityUser> FindUserAsync(string userName, string password)
    {
        return _repository.FindUserAsync(userName, password);
    }

    /// <summary>
    ///     Creates the role.
    /// </summary>
    /// <param name="roleName">Name of the role.</param>
    /// <returns></returns>
    public async Task<IdentityResult> CreateRole(string roleName)
    {
        return await _repository.CreateRole(roleName);
    }

    /// <summary>
    ///     Finds the user.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns></returns>
    public async Task<IdentityUser> FindUser(string username, string password)
    {
        return await _repository.FindUser(username, password);
    }

    /// <summary>
    ///     Finds the user.
    /// </summary>
    /// <param name="userName">Name of the user.</param>
    /// <returns></returns>
    public IdentityUser FindUser(string userName)
    {
        return _repository.FindUser(userName);
    }

    /// <summary>
    ///     Gets the user identifier by ASP identifier.
    /// </summary>
    /// <param name="aspId">The ASP identifier.</param>
    /// <returns></returns>
    public int GetUserIdByAspId(string aspId)
    {
        return _repository.GetUserByAspId(aspId).Id;
    }

    /// <summary>
    ///     Gets the user email by identifier.
    /// </summary>
    public string GetUserEmailById(int userId)
    {
        return _repository.GetById(userId).Email;
    }

    public List<User> GetAll()
    {
        return _repository.GetAll().ToList();
    }
}
﻿using Basecode.Data.Interfaces;
using Basecode.Data.Models;

namespace Basecode.Data.Repositories;

public class UserScheduleRepository : BaseRepository, IUserScheduleRepository
{
    private readonly BasecodeContext _context;

    public UserScheduleRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    /// <summary>
    ///     Creates a UserSchedule.
    /// </summary>
    /// <param name="userSchedule"></param>
    /// <returns></returns>
    public int AddUserSchedule(UserSchedule userSchedule)
    {
        _context.UserSchedule.Add(userSchedule);
        _context.SaveChanges();
        return userSchedule.Id;
    }

    /// <summary>
    ///     Inserts multiple UserSchedul records into the database.
    /// </summary>
    public void AddUserSchedules(List<UserSchedule> userSchedules)
    {
        _context.UserSchedule.AddRange(userSchedules);
        _context.SaveChanges();
    }

    /// <summary>
    ///     Gets the user schedule by identifier.
    /// </summary>
    /// <param name="userScheduleId">The user schedule identifier.</param>
    /// <returns></returns>
    public UserSchedule? GetUserScheduleById(int userScheduleId)
    {
        return _context.UserSchedule.Find(userScheduleId);
    }

    /// <summary>
    ///     Updates the specified user schedule.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    public void UpdateUserSchedule(UserSchedule userSchedule)
    {
        _context.UserSchedule.Update(userSchedule);
        _context.SaveChanges();
    }

    /// <summary>
    ///     Gets the identifier if user schedule exists.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    public int GetIdIfUserScheduleExists(Guid applicationId)
    {
        var id = _context.UserSchedule.FirstOrDefault(schedule => schedule.ApplicationId == applicationId);
        return id != null ? id.Id : -1;
    }

    public UserSchedule GetApplicationByGuid(Guid applicationId)
    {
        return _context.UserSchedule.FirstOrDefault(schedule => schedule.ApplicationId == applicationId);
    }

    /// <summary>
    ///     Deletes the user schedule.
    /// </summary>
    /// <param name="userSchedule">The user schedule.</param>
    public void DeleteUserSchedule(UserSchedule userSchedule)
    {
        _context.UserSchedule.Remove(userSchedule);
        _context.SaveChanges();
    }

    /// <summary>
    /// Gets the user schedule by application identifier.
    /// </summary>
    /// <param name="applicationId">The application identifier.</param>
    /// <returns></returns>
    public UserSchedule? GetUserScheduleByApplicationId(Guid applicationId)
    {
        return _context.UserSchedule.Where(schedule => schedule.ApplicationId == applicationId).FirstOrDefault();
    }
}
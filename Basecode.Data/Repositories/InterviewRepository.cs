using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using System;

namespace Basecode.Data.Repositories;

public class InterviewRepository : BaseRepository, IInterviewRepository
{
    private readonly BasecodeContext _context;

    public InterviewRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public IQueryable<Interview> GetAll()
    {
        return GetDbSet<Interview>();
    }

    public void AddInterview(Interview interview)
    {
        _context.Interview.Add(interview);
        _context.SaveChanges();
    }

    public Interview GetInterviewById(int id)
    {
        return _context.Interview.Find(id);
    }

    public void UpdateInterview(Interview interview)
    {
        _context.Interview.Update(interview);
        _context.SaveChanges();
    }

    public void DeleteInterview(Interview interview)
    {
        _context.Interview.Remove(interview);
        _context.SaveChanges();
    }

    public Interview? GetInterviewByApplicationIdAndStatus(Guid applicationId, string status)
    {
        return GetDbSet<Interview>()
            .FirstOrDefault(interview => interview.ApplicationId == applicationId && interview.Type == status);
    }
}
﻿using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace Basecode.Data.Repositories;

public class ApplicantRepository : BaseRepository, IApplicantRepository
{
    private readonly BasecodeContext _context;

    public ApplicantRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
    {
        _context = context;
    }

    public IQueryable<Applicant> GetAll()
    {
        return GetDbSet<Applicant>();
    }

    public Applicant GetById(int id)
    {
        return _context.Applicant.Find(id);
    }

    public Applicant GetByIdAll(int id)
    {
        return _context.Applicant
            .Where(a => a.Id == id)
            .Include(a => a.Application)
            .Include(a => a.Application.JobOpening)
            .FirstOrDefault();
    }

    public int CreateApplicant(Applicant applicant)
    {
        _context.Applicant.Add(applicant);
        _context.SaveChanges();

        return applicant.Id;
    }

    public IQueryable<Applicant> GetApplicantsByJobOpeningId(int jobOpeningId)
    {
        // Retrieve the applicants with their related applications' status
        var applicants = _context.Applicant
            .Where(applicant => applicant.Application.JobOpeningId == jobOpeningId);
        return applicants;
    }

    public Applicant GetApplicantByApplicationId(Guid applicationId)
    {
        var applicant = _context.Applicant
            .FirstOrDefault(applicant => applicant.Application.Id == applicationId);
        return applicant;
    }

    /// <summary>
    ///     Gets the linked job openings.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns></returns>
    public IEnumerable<JobOpeningBasicViewModel> GetLinkedJobOpenings(string userAspId)
    {
        return _context.User
            .Where(u => u.AspId == userAspId)
            .SelectMany(u => u.JobOpenings)
            .Select(j => new JobOpeningBasicViewModel
            {
                Id = j.Id,
                Title = j.Title
            })
            .ToList();
    }

    public List<Applicant> GetApplicantsWithJobAndReferences(string userAspId)
    {
        var linkedJobOpenings = GetLinkedJobOpenings(userAspId);
        var jobOpeningIds = linkedJobOpenings.Select(j => j.Id).ToList();

        var applicants = _context.Applicant
            .Where(applicant =>
                jobOpeningIds.Contains(applicant.Application.JobOpeningId)) // Include Application and JobOpening
            .Include(applicant => applicant.Application)
            .Include(applicant => applicant.Application.JobOpening)
            .Include(a => a.CharacterReferences)
            .ToList();

        return applicants;
    }

    /// <summary>
    /// Gets the applicants by status and job opening identifier.
    /// </summary>
    /// <param name="jobOpeningId">The job opening identifier.</param>
    /// <param name="status">The status.</param>
    /// <returns></returns>
    public IQueryable<Applicant> GetApplicantsByStatusAndJobOpeningId(int jobOpeningId, string status)
    {
        return _context.Applicant
                .Where(applicant => applicant.Application.JobOpeningId == jobOpeningId && applicant.Application.Status == status);
    }
}
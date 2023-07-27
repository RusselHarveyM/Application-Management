using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
    public class ApplicantRepository : BaseRepository, IApplicantRepository
    {
        private readonly BasecodeContext _context;

        public ApplicantRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public IQueryable<Applicant> GetAll()
        {
            return this.GetDbSet<Applicant>();
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

        public List<(string Name, string Email, string Title)> GetApplicantNameAndJobTitle()
        {
            var result = _context.Applicant
                .Join(_context.Application,
                    applicant => applicant.Id,
                    application => application.ApplicantId,
                    (applicant, application) => new { Applicant = applicant, Application = application })
                .Join(_context.JobOpening,
                    applicantApplication => applicantApplication.Application.JobOpeningId,
                    jobOpening => jobOpening.Id,
                    (applicantApplication, jobOpening) => new { ApplicantApplication = applicantApplication, JobOpening = jobOpening })
                .Select(joinedTables => new { joinedTables.ApplicantApplication.Applicant, joinedTables.JobOpening.Title })
                .ToList();

            var nameAndTitleList = result.Select(x => (Name: $"{x.Applicant.Firstname} {x.Applicant.Middlename} {x.Applicant.Lastname}".Trim(), x.Applicant.Email, x.Title)).ToList();
            return nameAndTitleList;
        }
    }
}

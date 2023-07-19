using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
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

        public int CreateApplicant(Applicant applicant)
        {
            _context.Applicant.Add(applicant);
            _context.SaveChanges();

            return applicant.Id;
        }

        public IQueryable<ApplicantStatusViewModel> GetApplicantsByJobOpeningId(int jobOpeningId)
        {
            // Retrieve the applicants with their related applications' status
            var applicants = _context.Applicant
                .Where(applicant => applicant.Application.JobOpeningId == jobOpeningId)
                .Select(applicant => new ApplicantStatusViewModel
                {
                    Firstname = applicant.Firstname,
                    Lastname = applicant.Lastname,
                    Status = applicant.Application.Status // Retrieve the Status from the related Application
                });

            return applicants;
        }
    }
}

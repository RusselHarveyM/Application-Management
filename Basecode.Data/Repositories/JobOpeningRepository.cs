using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
    public class JobOpeningRepository : BaseRepository, IJobOpeningRepository
    {
        private readonly BasecodeContext _context;

        public JobOpeningRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public IQueryable<JobOpening> GetAll()
        {
            return this.GetDbSet<JobOpening>();
        }

        public void AddJobOpening(JobOpening jobOpening)
        {
            _context.JobOpening.Add(jobOpening);
            _context.SaveChanges();
        }

        public JobOpening GetJobOpeningById(int id)
        {
            return _context.JobOpening.Find(id);
        }

        public void UpdateJobOpening(JobOpening jobOpening)
        {
            _context.JobOpening.Update(jobOpening);
            _context.SaveChanges();
        }

        public void DeleteJobOpening(JobOpening jobOpening)
        {
            _context.JobOpening.Remove(jobOpening);
            _context.SaveChanges();
        }
    }
}

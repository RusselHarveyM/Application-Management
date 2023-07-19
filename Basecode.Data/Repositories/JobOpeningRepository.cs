using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
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

        public List<int> GetAllJobOpeningIds()
        {
            return _context.JobOpening.Select(j => j.Id).ToList();
        }

        public IQueryable<JobOpening> GetJobsWithApplications()
        {
            return _context.JobOpening.Include(j => j.Applications);
        }

        public string GetJobOpeningTitleById(int id)
        {
            var title = _context.JobOpening
            .Where(j => j.Id == id)
            .Select(j => j.Title)
            .FirstOrDefault();

            return title;
        }

        public IQueryable<int> GetLinkedUserIds(int jobOpeningId)
        {
            return _context.JobOpening
                            .Where(j => j.Id == jobOpeningId)
                            .SelectMany(j => j.Users.Select(u => u.Id));
        }

        public void UpdateJobOpeningUsers(int jobOpeningId, List<int> assignedUserIds)
        {
            var jobOpening = _context.JobOpening
                .Include(j => j.Users)
                .FirstOrDefault(j => j.Id == jobOpeningId);

            if (jobOpening != null)
            {
                var existingUserIds = jobOpening.Users.Select(u => u.Id).ToList();

                // Find the users to remove from the junction table
                var usersToRemove = jobOpening.Users.Where(u => existingUserIds.Contains(u.Id) && !assignedUserIds.Contains(u.Id)).ToList();

                // Remove unassigned users from the junction table
                foreach (var user in usersToRemove)
                {
                    jobOpening.Users.Remove(user);
                }

                // Link the selected users to the JobOpening
                var selectedUsersToAdd = _context.User.Where(u => assignedUserIds.Contains(u.Id) && !existingUserIds.Contains(u.Id)).ToList();
                foreach (var user in selectedUsersToAdd)
                {
                    jobOpening.Users.Add(user);
                }

                _context.SaveChanges();
            }
        }
    }
}

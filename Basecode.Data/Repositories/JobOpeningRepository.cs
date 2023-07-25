using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

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

        public int AddJobOpening(JobOpening jobOpening)
        {
            _context.JobOpening.Add(jobOpening);
            _context.SaveChanges();
            return jobOpening.Id;
        }

        public JobOpening GetJobOpeningById(int id)
        {
            return _context.JobOpening.Include(j => j.Users).FirstOrDefault(j => j.Id == id);
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

        public IQueryable<string> GetLinkedUserIds(int jobOpeningId)
        {
            return _context.JobOpening
                            .Where(j => j.Id == jobOpeningId)
                            .SelectMany(j => j.Users.Select(u => u.AspId));
        }

        public void UpdateJobOpeningUsers(int jobOpeningId, List<string> assignedUserIds)
        {
            var jobOpening = _context.JobOpening
                .Include(j => j.Users)
                .FirstOrDefault(j => j.Id == jobOpeningId);

            if (jobOpening != null)
            {
                var existingUserIds = jobOpening.Users.Select(u => u.AspId).ToList();

                // Find the users to remove from the junction table
                var usersToRemove = jobOpening.Users.Where(u => existingUserIds.Contains(u.AspId) && !assignedUserIds.Contains(u.AspId)).ToList();

                // Remove unassigned users from the junction table
                foreach (var user in usersToRemove)
                {
                    jobOpening.Users.Remove(user);
                }

                // Link the selected users to the JobOpening
                var selectedUsersToAdd = _context.User.Where(u => assignedUserIds.Contains(u.AspId) && !existingUserIds.Contains(u.AspId)).ToList();
                foreach (var user in selectedUsersToAdd)
                {
                    jobOpening.Users.Add(user);
                }

                _context.SaveChanges();
            }
        }
    }
}

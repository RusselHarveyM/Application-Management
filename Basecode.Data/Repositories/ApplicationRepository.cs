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
    public class ApplicationRepository : BaseRepository, IApplicationRepository
    {
        private readonly BasecodeContext _context;

        public ApplicationRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public Guid CreateApplication(Application application)
        {
            _context.Application.Add(application);
            _context.SaveChanges();
            return application.Id;
        }

        public Application GetById(Guid id)
        {
            return _context.Application.Find(id);
        }

        public void UpdateApplication(Application application)
        {
            _context.Application.Update(application);
            _context.SaveChanges();
        }

        public List<Application> GetApplicationsByIds(List<Guid> applicationIds)
        {
            var applications = _context.Application
                .Where(a => applicationIds.Contains(a.Id)).ToList();
            return applications;
        }

        public Guid GetApplicationIdByApplicantId(int applicantId)
        {
            var application = _context.Application
                .FirstOrDefault(app => app.ApplicantId == applicantId);

            if (application != null)
            {
                return application.Id;
            }

            return Guid.Empty;
        }
    }
}

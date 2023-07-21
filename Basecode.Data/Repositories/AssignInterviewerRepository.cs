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
    public class AssignInterviewerRepository : BaseRepository, IAssignInterviewerRepository
    {
        private readonly BasecodeContext _context;

        public AssignInterviewerRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public IQueryable<AssignInterviewer> GetAll()
        {
            return this.GetDbSet<AssignInterviewer>();
        }

        public void AddAssign(AssignInterviewer assignInterviewer)
        {
            _context.AssignInterviewer.Add(assignInterviewer);
            _context.SaveChanges();
        }

        public AssignInterviewer GetJobAssignById(int id)
        {
            return _context.AssignInterviewer.Find(id);
        }
    }
}

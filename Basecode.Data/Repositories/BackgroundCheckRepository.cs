using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
    public class BackgroundCheckRepository : BaseRepository, IBackgroundCheckRepository
    {
        private readonly BasecodeContext _context;
        public BackgroundCheckRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        public int Create(BackgroundCheck form)
        {
            _context.BackgroundCheck.Add(form);
            _context.SaveChanges();
        }

        public IQueryable<BackgroundCheck> GetAll()
        {
            return this.GetDbSet<BackgroundCheck>();
        }

        public BackgroundCheck GetById(int id)
        {
            return _context.BackgroundCheck.Find(id);
        }
    }
}

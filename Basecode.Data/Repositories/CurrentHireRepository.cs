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
    public class CurrentHireRepository : BaseRepository, ICurrentHireRepository
    {
        private readonly BasecodeContext _context;

        public CurrentHireRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        /// <summary>
        /// Get current hire by Id
        /// </summary>
        /// <param name="currentHireId"></param>
        /// <returns></returns>
        public CurrentHire GetCurrentHireById(int currentHireId)
        {
            return _context.CurrentHire.Find(currentHireId);
        }

        /// <summary>
        /// Get current hire id by user id
        /// </summary>
        /// <param name="currentHireId"></param>
        /// <returns></returns>
        public int GetCurrentHireIdByUserId(int currentHireId)
        {
            var id = _context.CurrentHire.FirstOrDefault(offer => offer.UserId == currentHireId);
            return id != null ? id.Id : -1;
        }

        /// <summary>
        /// Get all current hire
        /// </summary>
        /// <returns></returns>
        public IQueryable<CurrentHire> GetAll()
        {
            return this.GetDbSet<CurrentHire>();
        }

        /// <summary>
        /// Update current hire
        /// </summary>
        /// <param name="currentHire"></param>
        public void UpdateCurrentHire(CurrentHire currentHire)
        {
            _context.CurrentHire.Update(currentHire);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update current hire
        /// </summary>
        /// <param name="userOffer"></param>
        //public void UpdateCurrentHires(CurrentHire currentHire)
        //{
        //    _context.CurrentHire.Update(currentHire);
        //    _context.SaveChanges();
        //}

        /// <summary>
        /// Add current hire
        /// </summary>
        /// <param name="currentHire"></param>
        /// <returns></returns>
        public int AddCurrentHire(CurrentHire currentHire)
        {
            _context.CurrentHire.Add(currentHire);
            _context.SaveChanges();
            return currentHire.Id;
        }

        /// <summary>
        /// Get id if current hire exists
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        public int GetIdIfCurrentHireExists(Guid applicationId)
        {
            var id = _context.CurrentHire.FirstOrDefault(offer => offer.ApplicationId == applicationId);
            return id != null ? id.Id : -1;
        }
    }
}

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
        /// Get user offer by Id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public CurrentHire GetUserOfferById(int userOfferId)
        {
            return _context.CurrentHire.Find(userOfferId);
        }

        /// <summary>
        /// Get user offer id by user id
        /// </summary>
        /// <param name="userOfferId"></param>
        /// <returns></returns>
        public int GetCurrentHireIdByUserId(int userOfferId)
        {
            var id = _context.CurrentHire.FirstOrDefault(offer => offer.UserId == userOfferId);
            return id != null ? id.Id : -1;
        }

        /// <summary>
        /// Get all useroffer
        /// </summary>
        /// <returns></returns>
        public IQueryable<UserOffer> GetAll()
        {
            return this.GetDbSet<UserOffer>();
        }
    }
}

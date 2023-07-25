using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly BasecodeContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="context">The context.</param>
        public UserRepository(IUnitOfWork unitOfWork, BasecodeContext context) : base(unitOfWork)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all users from the User table.
        /// </summary>
        /// <returns>
        /// A queryable collection of objects of type User.
        /// </returns>
        public IQueryable<User> RetrieveAll()
        {
            return this.GetDbSet<User>();
        }

        /// <summary>
        /// Adds a new user into the User table.
        /// </summary>
        /// <param name="user">Represents the user to be added.</param>
        public void Create(User user)
        {
            _context.User.Add(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Retrieves a user from the User table based on the specified ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>
        /// A User object corresponding to the matching ID.
        /// </returns>
        public User GetById(int id)
        {
            return _context.User.Find(id);
        }

        /// <summary>
        /// Updates an existing user in the User table.
        /// </summary>
        /// <param name="user">Represents the user with updated information.</param>
        public void Update(User user)
        {
            _context.User.Update(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public void Delete(User user)
        {
            _context.User.Remove(user);
            _context.SaveChanges();
        }
    }
}

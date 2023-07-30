using Basecode.Data.Interfaces;
using Basecode.Data.Models;
using Basecode.Data.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="context">The context.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public UserRepository(IUnitOfWork unitOfWork, BasecodeContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : base(unitOfWork)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task Create(User user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
            };
            identityUser.EmailConfirmed = true;
            var result = await _userManager.CreateAsync(identityUser, user.Password.ToString().Trim());

            if (result.Succeeded)
            {
                bool checkIfRoleExists = await _roleManager.RoleExistsAsync(user.Role);
                //If role not found then create and add user
                if (checkIfRoleExists)
                {
                    await _userManager.AddToRoleAsync(identityUser, user.Role);
                }
                else
                {
                    var createRole = await CreateRole(user.Role);
                    if (createRole.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(identityUser, user.Role);
                    }
                }
                user.AspId = identityUser.Id;
                // Add the new 'User' to your custom 'User' table
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    // Log or handle each error message as needed
                    Console.WriteLine($"Error Code: {error.Code}, Description: {error.Description}");
                }
                // Handle the case where user creation failed
                throw new Exception("Creation failed");
            }
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
        /// Gets the by identifier asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.User.FindAsync(id);
        }

        public User GetByEmail(string email)
        {
            return _context.User.FirstOrDefault(u => u.Email == email);
        }

        /// <summary>
        /// Updates an existing user in the User table.
        /// </summary>
        /// <param name="user">Represents the user with updated information.</param>
        public async Task Update(User user)
        {
            var identityUser = await _userManager.FindByIdAsync(user.AspId);
            var getUserToGetRole = await GetByIdAsync(user.Id);
            

            if (identityUser != null)
            {
                //Update identity user mail
                identityUser.Email = user.Email;
                identityUser.NormalizedEmail = user.Email.Normalize();
                identityUser.UserName = user.Email.ToLower();
                await _userManager.UpdateAsync(identityUser);
                //Update identity user password
                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                await _userManager.ResetPasswordAsync(identityUser, passwordToken, user.Password);
                //Update role
                await CreateRole(user.Role);
                var removeOldRole = await _userManager.RemoveFromRoleAsync(identityUser, getUserToGetRole.Role);
                if (removeOldRole.Succeeded)
                {
                    await _userManager.AddToRoleAsync(identityUser, user.Role);
                    getUserToGetRole.Email = user.Email;
                    getUserToGetRole.Username = user.Username;
                    getUserToGetRole.Role = user.Role;
                    getUserToGetRole.Password = user.Password;
                    await _context.SaveChangesAsync();
                }
            }

        }

        /// <summary>
        /// Deletes the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        public async Task Delete(User user)
        {
            var findUser = await _userManager.FindByIdAsync(user.AspId);
            if (findUser != null)
            {
                await _userManager.DeleteAsync(findUser);
            }

        }

        /// <summary>
        /// Gets the linked job openings.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IEnumerable<JobOpeningBasicViewModel> GetLinkedJobOpenings(string userAspId)
        {
            return _context.User
                   .Where(u => u.AspId == userAspId)
                   .SelectMany(u => u.JobOpenings)
                   .Select(j => new JobOpeningBasicViewModel
                   {
                       Id = j.Id,
                       Title = j.Title
                   })
                   .ToList();
        }

        /// <summary>
        /// Creates the role.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns></returns>
        public async Task<IdentityResult> CreateRole(string roleName)
        {
            bool checkIfRoleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!checkIfRoleExists)
            {
                var role = new IdentityRole();
                role.Name = roleName;
                var result = await _roleManager.CreateAsync(role);
                return result;
            }

            return null;
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        /// <summary>
        /// Finds the user asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public async Task<IdentityUser> FindUserAsync(string userName, string password)
        {
            var userDB = GetDbSet<IdentityUser>().Where(x => x.UserName.ToLower().Equals(userName.ToLower())).AsNoTracking().FirstOrDefault();
            var user = await _userManager.FindByNameAsync(userName);
            var isPasswordOK = await _userManager.CheckPasswordAsync(user, password);
            if ((user == null) || (isPasswordOK == false))
            {
                userDB = null;
            }
            return userDB;
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public IdentityUser FindUser(string userName)
        {
            var userDB = GetDbSet<IdentityUser>().Where(x => x.UserName.ToLower().Equals(userName.ToLower())).AsNoTracking().FirstOrDefault();
            return userDB;
        }

        /// <summary>
        /// Gets the user by ASP identifier.
        /// </summary>
        /// <param name="aspId">The ASP identifier.</param>
        /// <returns></returns>
        public User GetUserByAspId(string aspId)
        {
            return _context.User.Where(u => u.AspId == aspId).FirstOrDefault();
        }
    }
}

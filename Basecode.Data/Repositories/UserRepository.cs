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

        public User GetByEmail(string email)
        {
            return _context.User.FirstOrDefault(u => u.Email == email);
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

        /// <summary>
        /// Gets the linked job openings.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        public IEnumerable<JobOpeningBasicViewModel> GetLinkedJobOpenings(int userId)
        {
            return _context.User
                   .Where(u => u.Id == userId)
                   .SelectMany(u => u.JobOpenings)
                   .Select(j => new JobOpeningBasicViewModel
                   {
                       Id = j.Id,
                       Title = j.Title
                   })
                   .ToList();
        }

        public async Task RegisterUserToAsp(string username, string password, string email, string role)
        {
            var roleChecker = role == "Human Resources" ? "HR" : "DT";
            var user = new IdentityUser
            {
                UserName = username,
                Email = email,
            };

            await _userManager.CreateAsync(user, password);

            bool checkIfRoleExists = await _roleManager.RoleExistsAsync(roleChecker);
            if (checkIfRoleExists)
            {
                await _userManager.AddToRoleAsync(user, roleChecker);
            }
            //return await user.Id;
        }

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

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

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

        public IdentityUser FindUser(string userName)
        {
            var userDB = GetDbSet<IdentityUser>().Where(x => x.UserName.ToLower().Equals(userName.ToLower())).AsNoTracking().FirstOrDefault();
            return userDB;
        }
    }
}

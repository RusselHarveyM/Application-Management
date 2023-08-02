using Basecode.Data.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Basecode.Data.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> CreateRole(string roleName)
    {
        var checkIfRoleExists = await _roleManager.RoleExistsAsync(roleName);
        if (!checkIfRoleExists)
        {
            var role = new IdentityRole();
            role.Name = roleName;
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        return null;
    }
}
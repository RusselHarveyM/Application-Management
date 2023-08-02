using Microsoft.AspNetCore.Identity;

namespace Basecode.Data.Interfaces;

public interface IAdminRepository
{
    Task<IdentityResult> CreateRole(string roleName);
}
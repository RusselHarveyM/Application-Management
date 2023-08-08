using Microsoft.AspNetCore.Identity;

namespace Basecode.Services.Interfaces;

public interface IAdminService
{
    Task<IdentityResult> CreateRole(string roleName);
}
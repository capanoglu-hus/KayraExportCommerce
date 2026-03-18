using AuthApi.Dtos;

namespace AuthApi.Services
{
    public interface IRoleService
    {
        Task<string> Create(RoleDto role);
        Task<string> AssignRole(AssignRoleDto assignRole);
    }
}

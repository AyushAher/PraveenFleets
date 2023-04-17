using Shared.Configuration;
using Shared.Requests.Account;
using Shared.Responses.Account;

namespace Interfaces.Account;
public interface IRoleService : IService
{
    Task<ApiResponse<List<RoleResponse>>> GetAllAsync();
    Task<int> GetCountAsync();
    Task<ApiResponse<RoleResponse>> GetByIdAsync(Guid id);

    Task<BaseApiResponse> SaveAsync(RoleRequest request);
    Task<BaseApiResponse> DeleteAsync(Guid id);
    Task<ApiResponse<RoleResponse>> GetByName(string roleName);
    Task<ApiResponse<PermissionResponse>> GetAllPermissionsAsync(Guid roleId);

    Task<BaseApiResponse> UpdatePermissionsAsync(PermissionRequest request);
}
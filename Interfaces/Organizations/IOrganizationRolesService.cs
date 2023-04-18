using Shared.Configuration;
using Shared.Responses.Account;

namespace Interfaces.Organizations;

public interface IOrganizationRolesService
{
    Task<ApiResponse<bool>> UpSertUserRole(string role, UserResponse userResponse);
}
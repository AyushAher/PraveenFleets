using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace Interfaces.Organizations;

public interface IOrganizationRolesService: IService
{
    Task<ApiResponse<bool>> UpSertUserRole(CreateOrganizationRolesRequest request);
    
    Task<ApiResponse<OrganizationRoleResponse>> GetOrgRoleByUserId(Guid userId);

    Task<ApiResponse<List<OrganizationRoleResponse>>> GetOrgRoles(Guid organizationId);
}
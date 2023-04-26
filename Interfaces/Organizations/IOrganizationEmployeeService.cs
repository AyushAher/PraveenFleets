using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace Interfaces.Organizations;

public interface IOrganizationEmployeeService : IService
{
    Task<ApiResponse<OrganizationEmployeeResponse>> SaveOrganizationEmployee(OrganizationEmployeeRequest request);
    
    Task<ApiResponse<List<OrganizationEmployeeResponse>>> GetAllEmpolyees();
}
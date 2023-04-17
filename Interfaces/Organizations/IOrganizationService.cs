using Shared.Configuration;
using Shared.Requests.Organization;
using Shared.Responses.Organization;

namespace Interfaces.Organizations;

public interface IOrganizationService : IService
{
    Task<ApiResponse<OrganizationResponse>> RegisterOrganization(RegisterOrganization registerOrganizationRequest);
}
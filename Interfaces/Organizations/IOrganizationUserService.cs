using Shared.Configuration;
using Shared.Requests.Organization;

namespace Interfaces.Organizations;

public interface IOrganizationUserService : IService
{
    Task<BaseApiResponse> AddUserToOrganization(RegisterOrganizationUserRequest request);
}
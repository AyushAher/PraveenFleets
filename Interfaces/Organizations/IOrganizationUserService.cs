using Shared.Requests.Organization;

namespace Interfaces.Organizations;

public interface IOrganizationUserService : IService
{
    void AddUserToOrganization(RegisterOrganizationUserRequest request);
}
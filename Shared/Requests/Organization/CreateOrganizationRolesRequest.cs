using Shared.Responses.Account;

namespace Shared.Requests.Organization;

public class CreateOrganizationRolesRequest
{
    public string RoleName { get; set; } = string.Empty;
    public UserResponse User { get; set; }
}
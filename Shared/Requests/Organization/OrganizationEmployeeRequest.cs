using Shared.Requests.Account;

namespace Shared.Requests.Organization;

public class OrganizationEmployeeRequest:RegisterRequest
{
    public Guid UserId { get; set; }
}
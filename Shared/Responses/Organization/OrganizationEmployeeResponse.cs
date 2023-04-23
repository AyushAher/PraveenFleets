using Enums.Employee;
using Shared.Responses.Account;
using Shared.Responses.Common;

namespace Shared.Responses.Organization;

public class OrganizationEmployeeResponse : UserResponse
{
    public Guid UserId { get; set; }
    public Guid AddressId { get; set; }
    public AddressResponse Address { get; set; }
}
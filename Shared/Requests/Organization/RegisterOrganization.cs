using Shared.Requests.Account;
using Shared.Requests.Common;

namespace Shared.Requests.Organization;

public class RegisterOrganization
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public AddressRequest AddressRequest { get; set; }
    public RegisterRequest AdminDetailsRequest { get; set; }
}
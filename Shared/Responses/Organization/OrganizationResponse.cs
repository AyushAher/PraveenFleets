using Shared.Responses.Common;

namespace Shared.Responses.Organization;

public class OrganizationResponse
{
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public AddressResponse Address { get; set; }
    public Guid AdminUserId { get; set; }
}
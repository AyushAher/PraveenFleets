using Shared.Responses.Common;

namespace Shared.Responses.Organization;

public class OrganizationResponse
{
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public AddressResponse Address { get; set; }
    public Guid AdminId { get; set; }
    public Guid Id { get; set; }
    public Guid AddressId { get; set; }
}
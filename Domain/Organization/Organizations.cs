using Domain.Common;

namespace Domain.Organization;

public class Organizations : EntityTemplate<Guid>
{
    public string Name { get; set; }
    public string GstNumber { get; set; }
    public Guid AddressId { get; set; }
    public Guid AdminId { get; set; }
}
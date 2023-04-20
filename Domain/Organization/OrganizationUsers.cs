using Domain.Common;

namespace Domain.Organization;

public class OrganizationUsers : EntityTemplate<Guid>
{
    public Guid OrganizationId { get; set; }
    public Guid UserId { get; set; }
}
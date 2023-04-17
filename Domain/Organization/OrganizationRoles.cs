using Domain.Common;

namespace Domain.Organization;

public class OrganizationRoles : EntityTemplate<Guid>
{
    public Guid RoleId { get; set; }
    public Guid OrganizationId { get; set; }
}
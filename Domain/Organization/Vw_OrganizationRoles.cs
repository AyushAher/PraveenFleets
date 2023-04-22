using Domain.Common;

namespace Domain.Organization;

public class Vw_OrganizationRoles : EntityTemplate<Guid>
{
    public Guid RoleId { get; set; }
    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
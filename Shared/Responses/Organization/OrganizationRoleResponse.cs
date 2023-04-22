namespace Shared.Responses.Organization;

public class OrganizationRoleResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
}
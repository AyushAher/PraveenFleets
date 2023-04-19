namespace Shared.Responses.Organization;

public class OrganizationUserResponse
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid RoleId { get; set; }
}
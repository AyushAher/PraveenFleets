namespace Shared.Requests.Organization;

public class RegisterOrganizationUserRequest
{
    public Guid UserId { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid RoleId { get; set; }
}
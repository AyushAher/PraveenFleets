namespace Shared.Responses.Account;

public class GetAllRolesResponse
{
    public IEnumerable<RoleResponse> Roles { get; set; }
}
using Shared.Responses.Account;

namespace Shared.Requests.Account;
public class UpdateUserRolesRequest
{
    public Guid UserId { get; set; }

    public IList<UserRoleModel> UserRoles { get; set; }
}
using Shared.Requests.Account;

namespace Shared.Responses.Account;

public class PermissionRequest
{
    public Guid RoleId { get; set; } = Guid.Empty;
    public IList<RoleClaimRequest> RoleClaims { get; set; }
}
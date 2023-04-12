namespace Shared.Responses.Account;

public class PermissionResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public List<RoleClaimResponse> RoleClaims { get; set; }
}

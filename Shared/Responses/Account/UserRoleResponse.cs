namespace Shared.Responses.Account;

public class UserRoleModel
{
    public string RoleName { get; set; } = string.Empty;

    public string RoleDescription { get; set; } = string.Empty;

    public bool Selected { get; set; }
}


public class UserRolesResponse
{
    public List<UserRoleModel> UserRoles { get; set; } = new List<UserRoleModel>();
}
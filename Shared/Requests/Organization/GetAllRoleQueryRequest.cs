namespace Shared.Requests.Organization;

public class GetAllRoleQueryRequest
{
    public bool CheckRole { get; set; }
    public bool CheckOrganization { get; set; }
    public string ParamStr { get; set; } = string.Empty;
    public Guid Id { get; set; }
}
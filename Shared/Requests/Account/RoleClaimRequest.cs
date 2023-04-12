namespace Shared.Requests.Account;

public class RoleClaimRequest
{
    public int Id { get; set; }

    public Guid RoleId { get; set; } = Guid.Empty;

    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? Group { get; set; } = string.Empty;

    public bool Selected { get; set; }
}

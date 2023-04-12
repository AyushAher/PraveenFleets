using System.ComponentModel.DataAnnotations;

namespace Shared.Requests.Account;


public class RoleRequest
{
    public Guid Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    public string? Description { get; set; } = string.Empty;
}
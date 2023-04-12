using System.ComponentModel.DataAnnotations;

namespace Shared.Responses.Account;


public class RoleResponse
{
    public string Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}
using Domain.Common;
using Enums.Employee;

namespace Domain.Organization;

public class Vw_OrganizationEmployee : EntityTemplate<Guid>
{
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public Salutation Salutation { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
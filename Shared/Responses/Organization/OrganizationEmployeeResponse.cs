using Enums.Employee;
using Shared.Responses.Account;
using Shared.Responses.Common;

namespace Shared.Responses.Organization;

public class OrganizationEmployeeResponse
{
    public Guid Id { get; set; }
    public Salutation Salutation { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public string ContactNumber { get; set; }

    //List of Weekdays Enum converted to string
    public List<WeekDays> WeeklyOff { get; set; } = new();
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public AddressResponse Address { get; set; }
    public UserResponse User { get; set; }
    public Guid UserId { get; set; }
}
using Domain.Common;
using Enums.Employee;

namespace Domain.Organization;

public class OrganizationEmployee: EntityTemplate<Guid>
{
    public Salutation Salutation { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public string PhoneNumber { get; set; }
    
    //List of Weekdays Enum converted to string
    public string WeeklyOff { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid AddressId { get; set; }
    public Guid UserId { get; set; }
}
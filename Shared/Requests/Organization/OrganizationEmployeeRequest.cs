using System.ComponentModel.DataAnnotations;
using Enums.Employee;
using Shared.Requests.Common;

namespace Shared.Requests.Organization;

public class OrganizationEmployeeRequest
{

    public Guid Id { get; set; }
    
    [Required]
    public Salutation Salutation { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    
    public string MiddleName { get; set; }
    
    [Required]
    public string LastName { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public Gender Gender { get; set; }
    
    [Required]
    public string ContactNumber { get; set; }
    
    [Required]
    public string RoleId { get; set; }
    
    //List of Weekdays Enum converted to string
    public List<WeekDays> WeeklyOff { get; set; } = new();
    public Guid OrganizationId { get; set; }
    public AddressRequest AddressRequest { get; set; }
    public Guid UserId { get; set; }
}
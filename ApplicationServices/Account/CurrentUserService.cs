using System.Security.Claims;
using Enums.Account;
using Interfaces.Account;
using Microsoft.AspNetCore.Http;
using static System.String;

namespace ApplicationServices.Account;

public class CurrentUserService : ICurrentUserService
{
    /// <summary>
    /// Get Details of current LoggedIn User
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var claimsPrincipal = httpContextAccessor.HttpContext?.User;

        if (claimsPrincipal != null)
        {
            var usrIdentifier = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

            UserId = IsNullOrEmpty(usrIdentifier) ? Guid.Empty : new Guid(usrIdentifier);
            FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.Name);
            LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname);
            Email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        }
        else
        {
            UserId = Guid.Empty;
            FirstName = Empty;
            LastName = Empty;
            Email = Empty;
            CompanyId = Guid.Empty;
        }

        // Get All Claims
        Claims = claimsPrincipal?.Claims.AsEnumerable().Select(item =>
                 new KeyValuePair<string, string>(item.Type, item.Value))
                 .ToList();

        // Get All Roles
        UserRoles = Claims?.Where(c => c.Key == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
    }

    public Guid UserId { get; }

    public bool IsAdmin { get; } = false;

    public string FirstName { get; }

    public string LastName { get; }

    public string FullName => $"{FirstName} {LastName}";

    public string Email { get; }

    public Guid? CompanyId { get; }

    public List<string>? UserRoles { get; }

    public List<KeyValuePair<string, string>>? Claims { get; set; }
}
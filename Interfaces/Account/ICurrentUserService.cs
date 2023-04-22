using Enums.Account;

namespace Interfaces.Account
{
    public interface ICurrentUserService : IService
    {
        Guid UserId { get; }

        string FirstName { get; }

        string LastName { get; }

        string FullName { get; }

        string Email { get; }

        UserType UserType { get; }

        Guid ParentEntityId { get; }

        List<string> UserRoles { get; }
    }
}

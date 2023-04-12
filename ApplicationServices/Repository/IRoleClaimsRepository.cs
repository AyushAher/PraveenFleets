using Domain.Account;

namespace ApplicationServices.Repository;

public interface IRoleClaimsRepository
{
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<ApplicationRoleClaim> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<List<ApplicationRoleClaim>> GetAllByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default);

    Task<bool> CheckRoleCalimExistsAsync(
        Guid roleId,
        string claimType,
        string claimValue,
        CancellationToken cancellationToken = default);

    Task<List<ApplicationRoleClaim>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<ApplicationRoleClaim>> GetPagedResponseAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    Task<ApplicationRoleClaim> AddAsync(
        ApplicationRoleClaim entity,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(ApplicationRoleClaim entity,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int entity, CancellationToken cancellationToken = default);
}

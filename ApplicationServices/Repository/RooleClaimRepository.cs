using DB.Contexts;
using Domain.Account;
using Microsoft.EntityFrameworkCore;

namespace ApplicationServices.Repository;

public class RoleClaimsRepository : IRoleClaimsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoleClaimsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims.CountAsync(cancellationToken);
    }

    public async Task<ApplicationRoleClaim> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _dbContext.RoleClaims.FindAsync(id, cancellationToken);
    }

    public async Task<List<ApplicationRoleClaim>> GetAllByRoleIdAsync(Guid roleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckRoleCalimExistsAsync(Guid roleId, string claimType, string claimValue,
        CancellationToken cancellationToken = default)
    {
        var colResult = await _dbContext.RoleClaims.Where(x => x.RoleId == roleId
                                                               && x.ClaimType == claimType
                                                               && x.ClaimValue == claimValue)
            .ToListAsync(cancellationToken);

        if (colResult == null || colResult.Count == 0) return false;

        return true;
    }

    public async Task<List<ApplicationRoleClaim>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims.ToListAsync(cancellationToken);
    }

    public async Task<List<ApplicationRoleClaim>> GetPagedResponseAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RoleClaims
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<ApplicationRoleClaim> AddAsync(ApplicationRoleClaim entity,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RoleClaims.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(ApplicationRoleClaim entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var objRecord = await _dbContext.RoleClaims.FindAsync(entity.Id, cancellationToken);
            _dbContext.Entry(objRecord).CurrentValues.SetValues(entity);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var objRecord = await _dbContext.RoleClaims.FindAsync(id, cancellationToken);
            _dbContext.RoleClaims.Remove(objRecord);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}

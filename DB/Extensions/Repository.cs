using DB.Contexts;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DB.Extensions;
public class RepositoryAsync<T, TId> : IRepositoryAsync<T, TId> where T : EntityTemplate<TId>
{
    private readonly ApplicationDbContext _dbContext;

    public RepositoryAsync(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IQueryable<T> Entities => _dbContext.Set<T>();

    public Task<int> GetCount()
    {
        return Task.FromResult(Entities.Where(c => c.DeletedOn == null).Count());
    }

    public Task<int> GetCount(string sqlQuery)
    {
        using var objCommand = _dbContext.Database.GetDbConnection().CreateCommand();
        objCommand.CommandText = sqlQuery;
        objCommand.CommandType = System.Data.CommandType.Text;

        if (objCommand.Connection.State != System.Data.ConnectionState.Open)
            objCommand.Connection.Open();
        if (_dbContext.Database.CurrentTransaction != null)
        {
            objCommand.Transaction = _dbContext.Database.CurrentTransaction.GetDbTransaction();
        }

        var intCount = Convert.ToInt32(objCommand.ExecuteScalar());

        objCommand.Dispose();

        return Task.FromResult(intCount);
    }

    public async Task<T> GetByIdAsync(TId id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _dbContext
            .Set<T>()
            .Where(c => c.DeletedOn == null)
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(string sqlQuery)
    {
        return await _dbContext
            .Set<T>()
            .FromSqlRaw(sqlQuery)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public Task<bool> AddRangeAsync(List<T> entities)
    {
        _dbContext.Set<T>().AddRangeAsync(entities);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateAsync(T entity)
    {
        var exist = _dbContext.Set<T>().Find(entity.Id);
        _dbContext.Entry(exist).CurrentValues.SetValues(entity);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        return Task.FromResult(true);
    }

}


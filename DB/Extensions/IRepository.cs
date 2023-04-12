using Domain.Common;

namespace DB.Extensions;


public interface IRepositoryAsync<T, in TId> where T : class, IEntity<TId>
{
    IQueryable<T> Entities { get; }

    Task<int> GetCount();

    Task<int> GetCount(string sqlQuery);

    Task<T> GetByIdAsync(TId id);

    Task<List<T>> GetAllAsync();

    Task<List<T>> GetAllAsync(string sqlQuery);

    Task<T> AddAsync(T entity);

    Task<bool> AddRangeAsync(List<T> entity);

    Task<bool> UpdateAsync(T entity);

    Task<bool> DeleteAsync(T entity);
}
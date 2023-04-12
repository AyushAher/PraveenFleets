using System.Collections;
using DB.Contexts;
using Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace DB.Extensions;


public class UnitOfWork<TId> : IUnitOfWork<TId>
{
    private readonly ApplicationDbContext _dbContext;
    private bool _disposed;
    private readonly Hashtable _repositories;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _repositories ??= new Hashtable();
    }

    public IRepositoryAsync<TEntity, TId> Repository<TEntity>() where TEntity : EntityTemplate<TId>
    {
        var type = typeof(TEntity).Name;

        if (_repositories.ContainsKey(type)) return (IRepositoryAsync<TEntity, TId>)_repositories[type];

        var repositoryType = typeof(RepositoryAsync<,>);
        var repositoryInstance = Activator.CreateInstance(
            repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext);

        _repositories.Add(type, repositoryInstance);

        return (IRepositoryAsync<TEntity, TId>)_repositories[type];
    }

    public async Task<int> Save(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> StartTransaction(bool checkIfAlreadyExists = false)
    {
        if (!checkIfAlreadyExists) return await _dbContext.Database.BeginTransactionAsync();

        if (_dbContext.Database.CurrentTransaction != null)
            return _dbContext.Database.CurrentTransaction;

        return await _dbContext.Database.BeginTransactionAsync();
    }

    public Task Commit()
    {
        _dbContext.Database.CommitTransaction();
        return Task.CompletedTask;
    }

    public Task Rollback(bool handleOnlyIfExists = false)
    {
        if (handleOnlyIfExists && _dbContext.Database.CurrentTransaction == null)
            return Task.CompletedTask;

        _dbContext.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        _dbContext.Database.RollbackTransaction();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            //dispose managed resources
            _dbContext.Dispose();
        }

        //dispose unmanaged resources
        _disposed = true;
    }
}

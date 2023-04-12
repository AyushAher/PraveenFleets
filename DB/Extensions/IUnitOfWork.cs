using Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace DB.Extensions;


public interface IUnitOfWork<TId> : IDisposable
{
    IRepositoryAsync<T, TId> Repository<T>() where T : EntityTemplate<TId>;

    Task<int> Save(CancellationToken cancellationToken);

    Task<IDbContextTransaction> StartTransaction(bool checkIfAlreadyExists = false);

    Task Commit();

    Task Rollback(bool handleOnlyIfExists = false);
}
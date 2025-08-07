using _01_Data.Context;
using _01_Data.Entities.Base; 
using Microsoft.EntityFrameworkCore.Storage;

namespace _01_Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : BaseEntity;
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
    Task<int> SaveChangesAsync();
}

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    private IDbContextTransaction? _transaction;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (_repositories.TryGetValue(type, out var repo))
            return (IRepository<T>)repo;

        var repositoryInstance = new EfRepository<T>(context);
        _repositories[type] = repositoryInstance;
        return repositoryInstance;
    }

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();

    public async Task BeginTransactionAsync()
    {
        _transaction ??= await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}

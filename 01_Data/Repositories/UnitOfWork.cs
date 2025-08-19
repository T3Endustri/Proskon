// UnitOfWork.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _01_Data.Context;
using _01_Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace _01_Data.Repositories
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create();
    }

    /// <summary>
    /// IDbContextFactory ile thread-safe ve scope dışı DbContext üretimi.
    /// UI katmanında DbContext asla kullanılmaz; sadece Application üzerinden UoWFactory çağrılır.
    /// </summary>
    public sealed class UnitOfWorkFactory(IDbContextFactory<ApplicationDbContext> factory) : IUnitOfWorkFactory
    {
        public IUnitOfWork Create() => new UnitOfWork(factory);
    }

    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepository<T> Repository<T>() where T : BaseEntity;

        Task BeginTransactionAsync(CancellationToken ct = default);
        Task CommitTransactionAsync(CancellationToken ct = default);
        Task RollbackTransactionAsync(CancellationToken ct = default);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
        Task<bool> CanConnectAsync(CancellationToken ct = default);
    }

    /// <summary>
    /// Tek DbContext ömrü boyunca repository cache’ler.
    /// Transaction yönetimi sade ve güvenli: Begin/Commit/Rollback, dispose sırasında temizlik.
    /// </summary>
    public sealed class UnitOfWork(IDbContextFactory<ApplicationDbContext> factory) : IUnitOfWork
    {
        private readonly ApplicationDbContext _context = factory.CreateDbContext();
        private readonly Dictionary<Type, object> _repositories = [];
        private IDbContextTransaction? _transaction;

        /// <summary>
        /// Aynı entity için tek repository örneği tutar (per UoW).
        /// </summary>
        public IRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (_repositories.TryGetValue(type, out var repo))
                return (IRepository<T>)repo;

            var instance = new EfRepository<T>(_context);
            _repositories[type] = instance;
            return instance;
        }

        /// <summary>
        /// SaveChanges öncesi context’te değişiklik yoksa kısa devre yapar.
        /// </summary>
        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _context.ChangeTracker.HasChanges()
                ? _context.SaveChangesAsync(ct)
                : Task.FromResult(0);

        public async Task BeginTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null) return; // idempotent
            _transaction = await _context.Database.BeginTransactionAsync(ct);
        }

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return; // idempotent
            try
            {
                await _transaction.CommitAsync(ct);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is null) return; // idempotent
            try
            {
                await _transaction.RollbackAsync(ct);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public Task<bool> CanConnectAsync(CancellationToken ct = default)
            => _context.Database.CanConnectAsync(ct);

        public async ValueTask DisposeAsync()
        {
            // Artık açık transaction varsa güvenli şekilde bırak.
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            await _context.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}

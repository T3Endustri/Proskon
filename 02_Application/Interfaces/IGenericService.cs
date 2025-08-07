using _01_Data.Entities.Base;
using _01_Data.Specifications;
using System.Linq.Expressions;

namespace _02_Application.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    // ✅ Entity Erişimi (Internal Servis Kullanımı İçin)
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    // ✅ CRUD
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task DeleteAllAsync(Expression<Func<T, bool>> predicate);

    // ✅ Kontrol
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    // ✅ DTO Projection
    Task<List<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<List<TResult>> WhereSelectAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector);

    Task<(List<TResult> Items, int TotalCount)> PagingAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, object>>? orderBy,
        bool descending,
        int skip,
        int take);

    // ✅ Specification
    Task<List<T>> ListAsync(ISpecification<T> spec);

    // ✅ NEW — DTO Projection Overload'lar
    Task<TResult?> GetByIdAsync<TResult>(
        Guid id,
        Expression<Func<T, TResult>> selector);

    Task<List<TResult>> ListAsync<TResult>(
        ISpecification<T> spec,
        Expression<Func<T, TResult>> selector);
}

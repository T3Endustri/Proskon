using System.Linq.Expressions;
using _01_Data.Entities.Base;
using _01_Data.Specifications;

namespace _01_Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
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
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> ListAsync(ISpecification<T> spec);
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selector);
    Task<TResult?> GetByIdAsync<TResult>(
    Guid id,
    Expression<Func<T, TResult>> selector);

}

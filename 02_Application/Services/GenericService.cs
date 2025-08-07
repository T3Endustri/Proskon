using _01_Data.Entities.Base;
using _01_Data.Repositories;
using _01_Data.Specifications;
using _02_Application.Interfaces;
using System.Linq.Expressions;

namespace _02_Application.Services;

public class GenericService<T>(IUnitOfWork unitOfWork) : IGenericService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository = unitOfWork.Repository<T>();

    public Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        => _repository.GetByIdAsync(id, includes);

    public Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        => _repository.GetAllAsync(includes);

    public Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        => _repository.WhereAsync(predicate, includes);

    public async Task AddAsync(T entity)
    {
        await _repository.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
            await _repository.AddAsync(entity);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        await _repository.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAllAsync(Expression<Func<T, bool>> predicate)
    {
        var entities = await _repository.WhereAsync(predicate);
        foreach (var entity in entities)
            await _repository.DeleteAsync(entity.Id);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await _repository.WhereAsync(predicate);
        return result.Any();
    }

    public Task<List<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
    => _repository.SelectAsync(selector);

    public Task<List<TResult>> WhereSelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        => _repository.WhereSelectAsync(predicate, selector);
    public Task<(List<TResult> Items, int TotalCount)> PagingAsync<TResult>(
    Expression<Func<T, bool>> predicate,
    Expression<Func<T, TResult>> selector,
    Expression<Func<T, object>>? orderBy,
    bool descending,
    int skip,
    int take)
    => _repository.PagingAsync(predicate, selector, orderBy, descending, skip, take);

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    => _repository.CountAsync(predicate);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => _repository.AnyAsync(predicate);

    public Task<List<T>> ListAsync(ISpecification<T> spec)
    => _repository.ListAsync(spec);

    public Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector)
    => _repository.GetByIdAsync(id, selector);

    public Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selector)
        => _repository.ListAsync(spec, selector);

}
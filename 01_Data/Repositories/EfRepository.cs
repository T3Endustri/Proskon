using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using _01_Data.Entities.Base;
using _01_Data.Context;
using _01_Data.Specifications;

namespace _01_Data.Repositories;

public class EfRepository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        foreach (var include in includes)
            query = query.Include(include);

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        foreach (var include in includes)
            query = query.Include(include);

        var result = await query.ToListAsync();
        return [.. result];
    }

    public async Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);
        foreach (var include in includes)
            query = query.Include(include);

        var result = await query.ToListAsync();
        return [.. result];
    }

    public async Task AddAsync(T entity)
    {
        context.Entry(entity).State = EntityState.Added;
        await _dbSet.AddAsync(entity);
    }

    public Task UpdateAsync(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is not null)
            _dbSet.Remove(entity);
    }

    public async Task<List<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        var result = await _dbSet.AsNoTracking().Select(selector).ToListAsync();
        return [.. result];
    }

    public async Task<List<TResult>> WhereSelectAsync<TResult>(
        Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selector)
    {
        var result = await _dbSet.AsNoTracking()
                                 .Where(predicate)
                                 .Select(selector)
                                 .ToListAsync();
        return [.. result];
    }
    public async Task<(List<TResult> Items, int TotalCount)> PagingAsync<TResult>(
    Expression<Func<T, bool>> predicate,
    Expression<Func<T, TResult>> selector,
    Expression<Func<T, object>>? orderBy,
    bool descending,
    int skip,
    int take)
    {
        IQueryable<T> query = _dbSet.AsNoTracking().Where(predicate);

        if (orderBy is not null)
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        int totalCount = await query.CountAsync();
        List<TResult> items = await query
            .Skip(skip)
            .Take(take)
            .Select(selector)
            .ToListAsync();

        return ([.. items], totalCount);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    => _dbSet.AsNoTracking().CountAsync(predicate);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.AsNoTracking().AnyAsync(predicate);

    public async Task<List<T>> ListAsync(ISpecification<T> spec)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        if (spec.OrderBy is not null)
            query = spec.IsDescending ? query.OrderByDescending(spec.OrderBy) : query.OrderBy(spec.OrderBy);

        if (spec.Skip.HasValue)
            query = query.Skip(spec.Skip.Value);
        if (spec.Take.HasValue)
            query = query.Take(spec.Take.Value);

        var result = await query.ToListAsync();
        return [.. result];
    }

    public async Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selector)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (spec.Criteria is not null)
            query = query.Where(spec.Criteria);

        foreach (var include in spec.Includes)
            query = query.Include(include);

        if (spec.OrderBy is not null)
            query = spec.IsDescending
                ? query.OrderByDescending(spec.OrderBy)
                : query.OrderBy(spec.OrderBy);

        if (spec.Skip.HasValue)
            query = query.Skip(spec.Skip.Value);
        if (spec.Take.HasValue)
            query = query.Take(spec.Take.Value);

        return await query.Select(selector).ToListAsync();
    }

}

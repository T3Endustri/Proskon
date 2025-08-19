using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using _01_Data.Context;
using _01_Data.Specifications;
using _01_Data.Entities;

namespace _01_Data.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

    Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(Guid id, bool asNoTracking, bool useIdentityResolution,
                          params Expression<Func<T, object>>[] includes); 
    Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id); 
    Task AddRangeAsync(IEnumerable<T> entities);
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate);


    Task<List<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector);
    Task<List<TResult>> WhereSelectAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

    Task<(List<TResult> Items, int TotalCount)> PagingAsync<TResult>(Expression<Func<T, bool>> predicate,
                                                                     Expression<Func<T, TResult>> selector,
                                                                     Expression<Func<T, object>>? orderBy,
                                                                     bool descending,
                                                                     int skip,
                                                                     int take);

    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    Task<List<T>> ListAsync(ISpecification<T> spec);
    Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selector);

    Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector);

    IQueryable<T> Query();
    IQueryable<T> Query(ISpecification<T> spec);
}

public class EfRepository<T>(ApplicationDbContext context) : IRepository<T> where T : BaseEntity
{
    // Hot-path: Id'ye göre tekil çekimlerde plan/alloc giderini düşürür.
    private static class CQ
    {
        public static readonly Func<ApplicationDbContext, Guid, IAsyncEnumerable<T>> ById_NoTracking =
            EF.CompileAsyncQuery((ApplicationDbContext db, Guid id) =>
                db.Set<T>()
                  .AsNoTracking()
                  .Where(e => e.Id == id));

        public static readonly Func<ApplicationDbContext, Guid, IAsyncEnumerable<T>> ById_Tracking =
            EF.CompileAsyncQuery((ApplicationDbContext db, Guid id) =>
                db.Set<T>()
                  .Where(e => e.Id == id));
    }

    private readonly DbSet<T> _dbSet = context.Set<T>();

    public IQueryable<T> Query() => _dbSet.AsNoTracking();

    public IQueryable<T> Query(ISpecification<T> spec) => ApplySpec(_dbSet, spec);

    public async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = true, params Expression<Func<T, object>>[] includes)
    {
        // Include yoksa compiled query kullan
        if (includes is null || includes.Length == 0)
        {
            if (asNoTracking)
            {
                await foreach (var e in CQ.ById_NoTracking(context, id))
                    return e;
                return null;
            }
            else
            {
                await foreach (var e in CQ.ById_Tracking(context, id))
                    return e;
                return null;
            }
        }

        // Include varsa dinamik plan kaçınılmaz → normal yol
        IQueryable<T> q = _dbSet.AsQueryable();

        if (asNoTracking)
        {
            // Self-referans döngülerinde güvenli: identity resolution
            q = q.AsNoTrackingWithIdentityResolution();
        }
        else
        {
            q = q.AsTracking();
        }

        q = q.Where(x => x.Id == id);

        foreach (var include in includes)
            q = q.Include(include);

        // Çoklu include'da kartesyen patlamayı önle
        q = q.AsSplitQuery();

        return await q.FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id, bool asNoTracking, bool useIdentityResolution,
                                       params Expression<Func<T, object>>[] includes)
    {
        // Include yoksa compiled query’leri kullan
        if (includes is null || includes.Length == 0)
        {
            if (asNoTracking)
            {
                await foreach (var e in CQ.ById_NoTracking(context, id))
                    return e;
                return null;
            }
            else
            {
                await foreach (var e in CQ.ById_Tracking(context, id))
                    return e;
                return null;
            }
        }

        // Include varsa dinamik yol
        IQueryable<T> q = _dbSet.AsQueryable();

        if (asNoTracking)
            q = useIdentityResolution ? q.AsNoTrackingWithIdentityResolution()
                                      : q.AsNoTracking();
        else
            q = q.AsTracking();

        q = q.Where(x => x.Id == id);

        foreach (var include in includes)
            q = q.Include(include);

        // Çoklu include’da cartesian patlamayı önle
        q = q.AsSplitQuery();

        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> q = _dbSet.AsNoTracking();
        if (includes is { Length: > 0 })
        {
            foreach (var include in includes)
                q = q.Include(include);

            if (includes.Length > 1)
                q = q.AsSplitQuery();
        }

        var list = await q.ToListAsync();
        return [.. list];
    }

    public async Task<IReadOnlyList<T>> WhereAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> q = _dbSet.AsNoTracking().Where(predicate);

        if (includes is { Length: > 0 })
        {
            foreach (var include in includes)
                q = q.Include(include);

            if (includes.Length > 1)
                q = q.AsSplitQuery();
        }

        var list = await q.ToListAsync();
        return [.. list];
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
        _ = await _dbSet.Where(e => e.Id == id).ExecuteDeleteAsync(); 
    }
    public Task AddRangeAsync(IEnumerable<T> entities)
    => _dbSet.AddRangeAsync(entities);

    public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.Where(predicate).ExecuteDeleteAsync();

    public async Task<List<TResult>> SelectAsync<TResult>(Expression<Func<T, TResult>> selector)
    {
        var list = await _dbSet.AsNoTracking().Select(selector).ToListAsync();
        return [.. list];
    }

    public async Task<List<TResult>> WhereSelectAsync<TResult>(Expression<Func<T, bool>> predicate,
                                                               Expression<Func<T, TResult>> selector)
    {
        var list = await _dbSet.AsNoTracking()
                               .Where(predicate)
                               .Select(selector)
                               .ToListAsync();
        return [.. list];
    }

    public async Task<(List<TResult> Items, int TotalCount)> PagingAsync<TResult>(Expression<Func<T, bool>> predicate,
                                                                                  Expression<Func<T, TResult>> selector,
                                                                                  Expression<Func<T, object>>? orderBy,
                                                                                  bool descending,
                                                                                  int skip,
                                                                                  int take)
    {
        IQueryable<T> baseQ = _dbSet.AsNoTracking().Where(predicate);

        if (orderBy is not null)
            baseQ = descending ? baseQ.OrderByDescending(orderBy) : baseQ.OrderBy(orderBy);

        // Count ve sayfa verisini ayrı al: deadlock ve devasa tek sorgudan kaçın
        int total = await baseQ.CountAsync();

        var items = await baseQ
            .Skip(skip)
            .Take(take)
            .Select(selector)   // projection-first
            .ToListAsync();

        return ([.. items], total);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.AsNoTracking().CountAsync(predicate);

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => _dbSet.AsNoTracking().AnyAsync(predicate);

    public async Task<List<T>> ListAsync(ISpecification<T> spec)
    {
        var q = ApplySpec(_dbSet, spec);
        return await q.ToListAsync();
    }

    public async Task<TResult?> GetByIdAsync<TResult>(Guid id, Expression<Func<T, TResult>> selector)
    {
        // projection-first
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(selector)
            .FirstOrDefaultAsync();
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<T> spec, Expression<Func<T, TResult>> selector)
    {
        var q = ApplySpec(_dbSet, spec);
        return await q.Select(selector).ToListAsync();
    }

    public async Task<T?> FirstOrDefaultAsync(ISpecification<T> spec,
                                              Expression<Func<T, bool>> predicate,
                                              CancellationToken ct = default)
    {
        var q = ApplySpec(_dbSet, spec);
        if (predicate is not null)
            q = q.Where(predicate);

        return await q.FirstOrDefaultAsync(ct);
    }

    public Task<T?> FirstOrDefaultAsync(ISpecification<T> spec, CancellationToken ct = default)
        => FirstOrDefaultAsync(spec, _ => true, ct);

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, ct);

    // ---- helpers ----

    private static IEnumerable<string> SafeIncludeStrings(ISpecification<T> spec)
    {
        // Specification.IncludeStrings tipi List<string> ya da string[] olabilir.
        // Her iki durumu da güvenli şekilde enumerate edelim.
        if (spec.IncludeStrings is null) yield break;

        if (spec.IncludeStrings is string[] arr)
        {
            foreach (var s in arr) if (!string.IsNullOrWhiteSpace(s)) yield return s;
        }
        else if (spec.IncludeStrings is IEnumerable<string> en)
        {
            foreach (var s in en) if (!string.IsNullOrWhiteSpace(s)) yield return s;
        }
    }

    private static IQueryable<T> ApplySpec(IQueryable<T> q, ISpecification<T> spec)
    {
        if (spec is null) return q;

        if (spec.Criteria is not null)
            q = q.Where(spec.Criteria);

        // Includes
        foreach (var inc in spec.Includes)
            q = q.Include(inc);

        foreach (var incStr in SafeIncludeStrings(spec))
            q = q.Include(incStr);

        // OrderBy / ThenBy
        if (spec.OrderBy is not null)
            q = spec.IsDescending ? q.OrderByDescending(spec.OrderBy) : q.OrderBy(spec.OrderBy);

        if (spec.ThenBys is { Count: > 0 })
        {
            var alreadyOrdered = q as IOrderedQueryable<T>;
            var ordered = alreadyOrdered ?? q.OrderBy(spec.ThenBys[0]);

            // Eğer daha önce orderlandıysa tüm ThenBy'ları uygula; değilse ilkini kullandık, kalanları uygula
            int start = alreadyOrdered is null ? 1 : 0;
            for (int i = start; i < spec.ThenBys.Count; i++)
                ordered = ordered.ThenBy(spec.ThenBys[i]);

            q = ordered;
        }

        // Çoklu include için önerilen split
        if (spec.UseSplitQuery || spec.AllowCycleIncludes)
            q = q.AsSplitQuery();

        // Tracking / No-tracking & IdentityResolution
        if (spec.IsReadOnlyQuery)
        {
            // Döngüye bilerek izin verilen senaryoda identity resolution zorunlu
            if (spec.AllowCycleIncludes)
                q = q.AsNoTrackingWithIdentityResolution();
            else
                q = spec.UseIdentityResolution
                    ? q.AsNoTrackingWithIdentityResolution()
                    : q.AsNoTracking();
        }
        else
        {
            q = q.AsTracking();
        }

        if (spec.Skip.HasValue) q = q.Skip(spec.Skip.Value);
        if (spec.Take.HasValue) q = q.Take(spec.Take.Value);

        return q;
    }
}

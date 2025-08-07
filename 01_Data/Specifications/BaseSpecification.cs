using System.Linq.Expressions;
namespace _01_Data.Specifications;

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; set; }
    public Expression<Func<T, object>>[] Includes { get; set; } = [];
    public Expression<Func<T, object>>? OrderBy { get; set; }
    public bool IsDescending { get; set; }
    public int? Skip { get; set; }
    public int? Take { get; set; }
}
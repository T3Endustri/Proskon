using System.Linq.Expressions;

namespace _01_Data.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>[] Includes { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    bool IsDescending { get; }
    int? Skip { get; }
    int? Take { get; }
}
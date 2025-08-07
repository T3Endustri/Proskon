using System.Linq.Expressions;

namespace _01_Data.Specifications;

public class InlineSpec<T> : BaseSpecification<T>
{
    public InlineSpec(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public InlineSpec() : this(_ => true)
    {
    }

    public InlineSpec(params Expression<Func<T, object>>[] includes) : this(_ => true)
    {
        Includes = includes;
    }
}
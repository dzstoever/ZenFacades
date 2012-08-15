
namespace Zen.Data.QueryModel
{
    public enum CriteriaOperators
    {
        /// <summary>
        /// Logical conjunction we use to create "Junction" groups</summary>
        And,
        /// <summary>
        /// Logical disjuction we use to create "Junction" groups</summary>
        Or,
        Equal,
        NotEqual,
        GreaterThan,
        LesserThan,
        GreaterThanOrEqual,
        LesserThanOrEqual,
        Like,
        NotLike,
        IsNull,
        IsNotNull,
        In,
        NotIn
    }
}
using System.Linq.Expressions;

namespace M.Simple.Extensions.Expressions;

/// <summary>可供 IQueryable 使用的条件表达式组合，不生成 Invoke 节点。</summary>
public static class PredicateExtensions
{
    /// <summary>创建恒真的条件。</summary>
    public static Expression<Func<T, bool>> True<T>() => value => true;

    /// <summary>创建恒假的条件。</summary>
    public static Expression<Func<T, bool>> False<T>() => value => false;

    /// <summary>以短路或组合两个条件；任一条件为空时返回另一条件。</summary>
    public static Expression<Func<T, bool>>? Or<T>(this Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>>? right) => Combine(left, right, Expression.OrElse);

    /// <summary>以短路与组合两个条件；任一条件为空时返回另一条件。</summary>
    public static Expression<Func<T, bool>>? And<T>(this Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>>? right) => Combine(left, right, Expression.AndAlso);

    /// <summary>创建成员值属于指定集合的条件；空集合匹配不到任何记录。</summary>
    public static Expression<Func<T, bool>> In<T, TValue>(this Expression<Func<T, TValue>> member,
        IEnumerable<TValue> values)
    {
        ArgumentNullException.ThrowIfNull(member);
        ArgumentNullException.ThrowIfNull(values);
        var contains = Expression.Call(typeof(Enumerable), nameof(Enumerable.Contains), [typeof(TValue)],
            Expression.Constant(values.ToArray()), member.Body);
        return Expression.Lambda<Func<T, bool>>(contains, member.Parameters);
    }

    private static Expression<Func<T, bool>>? Combine<T>(Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>>? right, Func<Expression, Expression, BinaryExpression> operation)
    {
        if (left == null) return right;
        if (right == null) return left;
        // 统一参数实例，避免表达式引用未绑定参数，并保留短路求值。
        var body = new ParameterReplaceVisitor(right.Parameters[0], left.Parameters[0]).Visit(right.Body)!;
        return Expression.Lambda<Func<T, bool>>(operation(left.Body, body), left.Parameters);
    }

    private sealed class ParameterReplaceVisitor(ParameterExpression source, ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) => node == source ? target : base.VisitParameter(node);
    }
}

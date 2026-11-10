using System.Linq.Expressions;
using M.Simple.Extensions.Expressions;

namespace M.Simple.Tests;

[TestClass]
public class ExpressionExtensionsTests
{
    [TestMethod]
    public void CombiningDifferentParameterInstancesSupportsShortCircuiting()
    {
        Expression<Func<string, bool>> exists = left => left != null;
        Expression<Func<string, bool>> startsWith = right => right.StartsWith("M.");
        var combined = exists.And(startsWith);
        Assert.IsFalse(combined.Compile()(null));
        Assert.IsTrue(combined.Compile()("M.Simple"));
        Assert.IsFalse(combined.ToString().Contains("Invoke"));

        Expression<Func<string, bool>> missing = left => left == null;
        Assert.IsTrue(missing.Or(startsWith).Compile()(null));
    }

    [TestMethod]
    public void NullPredicatesAndEmptyInHaveExplicitSemantics()
    {
        Expression<Func<int, bool>> empty = null;
        var always = PredicateExtensions.True<int>();
        Assert.AreSame(always, empty.And(always));
        Assert.AreSame(always, always.Or(empty));
        Assert.IsNull(empty.Or(empty));
        Assert.IsFalse(PredicateExtensions.False<int>().Compile()(1));
        Expression<Func<int, int>> value = number => number;
        Assert.IsFalse(value.In(Array.Empty<int>()).Compile()(1));
        Assert.IsTrue(value.In(new[] { 1, 2 }).Compile()(2));
    }
}

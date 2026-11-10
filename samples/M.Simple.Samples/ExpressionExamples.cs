using System.Linq.Expressions;
using M.Simple.Extensions.Expressions;

namespace M.Simple.Samples;

public static class ExpressionExamples
{
    public static void Run()
    {
        Expression<Func<Product, bool>> active = row => row.Active;
        Expression<Func<Product, int>> id = row => row.Id;
        var filter = active.And(id.In(new[] { 1, 3 }))!;
        var rows = new[]
        {
            new Product { Id = 1, Active = true },
            new Product { Id = 2, Active = true },
            new Product { Id = 3, Active = false }
        }.AsQueryable().Where(filter).ToArray();
        Verify.That(rows.Length == 1 && rows[0].Id == 1, "表达式组合结果错误");
        Console.WriteLine($"符合条件的产品：{rows[0].Id}");
    }
}

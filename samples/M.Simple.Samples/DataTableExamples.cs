using System.Collections;
using M.Simple.Extensions.DataTable;

namespace M.Simple.Samples;

public static class DataTableExamples
{
    public static void Run()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "产品 A", Price = 12m },
            new() { Id = 2, Name = "产品 B", Price = 30m }
        };
        using var table = ((IList)products).ToDataTable();
        using var filtered = table.Where(row => (decimal)row["Price"] >= 20m);
        var rows = filtered.ToLists<Product>();
        Verify.That(rows.Count == 1 && rows[0].Id == 2, "DataTable 转换结果错误");
        Console.WriteLine($"源表 {table.Rows.Count} 行；筛选后 {rows.Count} 行：{rows[0].Name}");
    }
}

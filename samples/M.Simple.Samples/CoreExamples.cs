using M.Simple.Extensions.Core;

namespace M.Simple.Samples;

public static class CoreExamples
{
    public static void Run()
    {
        // 匹配格式化与月份边界。
        var normalized = " （“样例”） ".ToFormatMatch();
        var month = "2024-02".ParseMonthToDateTime();
        Verify.That(normalized == "(\"样例\")" && month.EndOfMonth().Day == 29, "格式或月份结果错误");
        Console.WriteLine($"匹配文本：{normalized}；月末：{month.EndOfMonth():yyyy-MM-dd HH:mm:ss.fff}");

        // 分组默认保留第一条，安全字典默认保留最后一条。
        var products = new[]
        {
            new Product { Id = 1, Name = "原名称", Price = 12.345m },
            new Product { Id = 1, Name = "新名称", Price = 20m },
            new Product { Id = 2, Name = "另一产品", Price = 30m }
        };
        var first = products.GroupBySelectFirst(row => row.Id).ToArray();
        var last = products.ToDictionarySafe(row => row.Id);
        var batches = products.SplitBatch(2).Select(batch => batch.Count()).ToArray();
        Verify.That(first[0].Name == "原名称" && last[1].Name == "新名称", "重复键处理错误");
        Console.WriteLine($"分批：{string.Join(',', batches)}；金额：{12.345m.ToRound(2)}");

        // 接收者是新值，第二个参数是旧值；只比较标记 CompareValue 的属性。
        var changes = products[1].CompareModel(products[0], isAttr: true);
        var fields = typeof(Product).GetPublicPropertyNames();
        var dictionary = products[0].ToDictionary();
        Verify.That(changes.Count == 2 && fields.Contains("Name") && dictionary.ContainsKey("name"), "比较或反射结果错误");
        Console.WriteLine($"变化字段：{string.Join(',', changes.Select(change => change.FiledName))}");
        Console.WriteLine($"枚举：{OrderStatus.Pending.GetDisplayName()}；属性：{products[0].GetObjectPropertyValue("NAME")}");
    }
}

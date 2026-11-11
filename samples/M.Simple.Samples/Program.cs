using System.ComponentModel;
using M.Simple.Extensions.Collections;
using M.Simple.Extensions.Comparison;
using M.Simple.Extensions.Enums;
using M.Simple.Extensions.Strings;

var scene = args.FirstOrDefault() ?? "all";
if (scene is not ("all" or "enums" or "strings" or "collections" or "comparison"))
{
    Console.Error.WriteLine("可选参数：all | enums | strings | collections | comparison");
    return 1;
}
if (scene is "all" or "enums")
{
    Console.WriteLine("[枚举]");
    foreach (var option in EnumExtensions.GetOptions<AuditStatus>())
        Console.WriteLine($"{(int)option.Value}: {option.Text}");
}
if (scene is "all" or "strings")
{
    Console.WriteLine("[脱敏]");
    Console.WriteLine("13812345678".MaskPhone());
    Console.WriteLine("alice@example.com".MaskEmail());
    Console.WriteLine("1234567890".Mask(2, 2));
}
if (scene is "all" or "collections")
{
    Console.WriteLine("[集合]");
    var before = new[] { new Department(1, 0, "销售"), new Department(2, 1, "旧部门") };
    var after = new[] { new Department(1, 0, "营销"), new Department(3, 1, "新部门") };
    var diff = before.DiffBy(after, item => item.Id);
    Console.WriteLine($"新增 {diff.Added.Count}，修改 {diff.Updated.Count}，删除 {diff.Removed.Count}");
    var tree = after.ToTree(item => item.Id, item => item.ParentId, item => item.ParentId == 0);
    Console.WriteLine($"根节点：{tree[0].Value.Name}；子节点：{tree[0].Children[0].Value.Name}");
}
if (scene is "all" or "comparison")
{
    Console.WriteLine("[对象比较]");
    var differences = ObjectComparison.Compare(new Customer("张三", 100m), new Customer("李四", 120m));
    foreach (var difference in differences)
        Console.WriteLine($"{difference.DisplayName}：{difference.Before} → {difference.After}");
}
return 0;

enum AuditStatus { [Description("待审核")] Pending = 0, [Description("已通过")] Approved = 1 }
record Department(int Id, int ParentId, string Name);
record Customer([property: DisplayName("姓名")] string Name, [property: DisplayName("额度")] decimal Credit);

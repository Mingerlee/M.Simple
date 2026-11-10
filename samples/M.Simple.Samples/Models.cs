using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using M.Simple.Extensions.Core.Attributes;

namespace M.Simple.Samples;

public class Product
{
    public int Id { get; set; }
    [CompareValue]
    public string Name { get; set; } = string.Empty;
    [CompareValue]
    public decimal Price { get; set; }
    public bool Active { get; set; }
}

public enum OrderStatus
{
    [Description("待处理"), Display(Name = "待处理")]
    Pending = 1,
    [Description("已完成"), Display(Name = "已完成")]
    Completed = 2
}

internal static class Verify
{
    public static void That(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

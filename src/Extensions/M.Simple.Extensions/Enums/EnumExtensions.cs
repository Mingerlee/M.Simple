using System.ComponentModel;
using System.Reflection;

namespace M.Simple.Extensions.Enums;

/// <summary>保留枚举原始类型的选项。</summary>
public sealed record EnumOption<TEnum>(TEnum Value, string Text) where TEnum : struct, Enum;

/// <summary>枚举描述与选项扩展。</summary>
public static class EnumExtensions
{
    /// <summary>获取 Description；无描述、未定义值及未命名标志组合返回 ToString()。</summary>
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        var name = Enum.GetName(value);
        return name is null ? value.ToString() :
            typeof(TEnum).GetField(name)?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? name;
    }

    /// <summary>按枚举值顺序返回选项；别名按同一值合并，保留枚举类型而不强转 int。</summary>
    public static IReadOnlyList<EnumOption<TEnum>> GetOptions<TEnum>() where TEnum : struct, Enum
    {
        return Array.AsReadOnly(Enum.GetValues<TEnum>().Distinct()
            .Select(value => new EnumOption<TEnum>(value, value.GetDescription())).ToArray());
    }
}

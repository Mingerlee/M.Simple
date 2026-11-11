using System.ComponentModel;
using System.Reflection;

namespace M.Simple.Extensions.Comparison;

/// <summary>属性差异，包含属性名、显示名称及修改前后的原始值。</summary>
public sealed record PropertyDifference(string PropertyName, string DisplayName, object? Before, object? After);

/// <summary>对象浅比较选项。属性名区分大小写；配置对象不应在比较期间修改。</summary>
public sealed class ComparisonOptions
{
    /// <summary>不参与比较的属性名称。</summary>
    public ISet<string> IgnoredProperties { get; } = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>按属性配置相等判断；返回 true 表示没有差异。</summary>
    public IDictionary<string, Func<object?, object?, bool>> Comparers { get; }
        = new Dictionary<string, Func<object?, object?, bool>>(StringComparer.Ordinal);
}

/// <summary>比较声明类型的公共可读实例属性，不递归遍历引用对象。</summary>
public static class ObjectComparison
{
    /// <summary>比较两个非 null 对象；跳过索引器，默认使用 object.Equals，读取异常直接传递。</summary>
    public static IReadOnlyList<PropertyDifference> Compare<T>(T before, T after, ComparisonOptions? options = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(before);
        ArgumentNullException.ThrowIfNull(after);
        var differences = new List<PropertyDifference>();
        foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .OrderBy(property => property.Name, StringComparer.Ordinal))
        {
            // 避免访问索引器和非公开 getter；使用声明类型确保比较范围固定。
            if (property.GetMethod?.IsPublic != true || property.GetIndexParameters().Length != 0
                || options?.IgnoredProperties.Contains(property.Name) == true) continue;
            var oldValue = property.GetValue(before);
            var newValue = property.GetValue(after);
            var equal = options is not null && options.Comparers.TryGetValue(property.Name, out var comparer)
                ? comparer(oldValue, newValue) : Equals(oldValue, newValue);
            if (equal) continue;
            var display = property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                ?? property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name;
            differences.Add(new PropertyDifference(property.Name, display, oldValue, newValue));
        }
        return differences.AsReadOnly();
    }
}

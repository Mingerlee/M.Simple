using System.Reflection;

namespace M.Simple.Extensions.Core;

/// <summary>公共实例属性的发现、取值和字典转换。</summary>
public static class ReflectionExtensions
{
    /// <summary>返回公共实例属性名集合，按序号区分大小写。</summary>
    public static HashSet<string> GetPublicPropertyNames(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Select(property => property.Name).ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>将公共可读的非索引属性转换为字典；默认使用不依赖区域设置的小写键。</summary>
    public static IDictionary<string, object?> ToDictionary(this object value, bool lowerCaseKeys = true)
    {
        ArgumentNullException.ThrowIfNull(value);
        var result = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var property in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            // 索引器需要额外参数；非公共 getter 不属于可公开读取的属性。
            if (property.GetMethod?.IsPublic != true || property.GetIndexParameters().Length != 0) continue;
            result[lowerCaseKeys ? property.Name.ToLowerInvariant() : property.Name] = property.GetValue(value);
        }
        return result;
    }

    /// <summary>按属性名取值，忽略名称大小写；属性不存在时抛出 KeyNotFoundException。</summary>
    public static object? GetObjectPropertyValue(this object value, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        return value.ToDictionary()[propertyName.ToLowerInvariant()];
    }
}

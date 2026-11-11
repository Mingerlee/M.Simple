namespace M.Simple.Extensions.Strings;

/// <summary>字符串脱敏扩展。保留长度按 UTF-16 字符计数。</summary>
public static class StringExtensions
{
    /// <summary>保留首尾指定长度，其余替换为 mask；长度不足时全部遮盖，null 原样返回。</summary>
    public static string? Mask(this string? value, int visiblePrefix = 0, int visibleSuffix = 0, char mask = '*')
    {
        ArgumentOutOfRangeException.ThrowIfNegative(visiblePrefix);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleSuffix);
        if (string.IsNullOrEmpty(value)) return value;
        // 避免短字符串和重叠区间暴露完整信息，同时避免长度相加溢出。
        if (visiblePrefix >= value.Length || visibleSuffix >= value.Length - visiblePrefix)
            return new string(mask, value.Length);
        return value[..visiblePrefix] + new string(mask, value.Length - visiblePrefix - visibleSuffix)
            + value[(value.Length - visibleSuffix)..];
    }

    /// <summary>手机号显示前 3 后 4 位；不做格式验证，7 位及以下全部遮盖。</summary>
    public static string? MaskPhone(this string? value) => value.Mask(3, 4);

    /// <summary>遮盖邮箱用户名，保留首字符和域名；单字符用户名也遮盖，格式不符时全部遮盖。</summary>
    public static string? MaskEmail(this string? value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var at = value.IndexOf('@');
        if (at <= 0 || at == value.Length - 1 || at != value.LastIndexOf('@')) return value.Mask();
        return value[..at].Mask(at > 1 ? 1 : 0) + value[at..];
    }
}

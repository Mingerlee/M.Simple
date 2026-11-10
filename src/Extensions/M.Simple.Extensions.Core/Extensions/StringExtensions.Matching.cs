namespace M.Simple.Extensions.Core;

public static partial class StringExtensions
{
    /// <summary>判断字符串是否包含非空白内容。</summary>
    public static bool HasValue(this string? value) => !string.IsNullOrWhiteSpace(value);

    /// <summary>去除首尾空白，统一中英文括号和左右中文双引号；空白输入原样返回。</summary>
    public static string? ToFormatMatch(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return value;
        // 匹配前只统一约定符号，不改变大小写或中间空白。
        return value.Trim().Replace('（', '(').Replace('）', ')').Replace('“', '"').Replace('”', '"');
    }
}
